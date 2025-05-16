using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public enum SkeletonType {
        Human = 0,
        Monster = 1,
        DemiHuman = 2,
        Weapon = 3,
    }

    public class PapFile : FileManagerFile {
        public readonly string HkxTempLocation;
        public readonly string SourcePath;
        public bool IsMaterial => SourcePath?.Contains( "material.pap" ) == true || Animations.Any( x => x.GetPapType() == 22 );

        public readonly ParsedShort ModelId = new( "Model Id" );
        public readonly ParsedEnum<SkeletonType> ModelType = new( "Skeleton Type", size: 1 );
        public readonly ParsedInt Variant = new( "Variant", size: 1 );

        public readonly List<PapAnimation> Animations = [];
        public readonly PapAnimationDropdown AnimationsDropdown;
        public readonly PapMotions MotionData;

        // Pap files from mods sometimes get exported with a weird padding, so we have to account for that
        private readonly int ModdedTmbOffset4 = 0;
        private readonly int ModdedPapMod4 = 0;

        private readonly bool EmptyHavok = false;

        public readonly HashSet<nint> Handles = [];

        public PapFile( BinaryReader reader, string sourcePath, string hkxTemp, bool init, bool verify ) : base() {
            SourcePath = sourcePath;
            HkxTempLocation = hkxTemp;
            AnimationsDropdown = new( this, Animations );

            reader.ReadInt32(); // magic
            reader.ReadInt32(); // version
            var numAnimations = reader.ReadInt16();

            ModelId.Read( reader );
            ModelType.Read( reader );
            Variant.Read( reader );

            reader.ReadInt32(); // info offset
            var havokPosition = reader.ReadInt32(); // from beginning
            var footerPosition = reader.ReadInt32();

            for( var i = 0; i < numAnimations; i++ ) {
                Animations.Add( new PapAnimation( this, reader, HkxTempLocation ) );
            }

            var havokDataSize = footerPosition - havokPosition;
            reader.BaseStream.Position = havokPosition;
            var havokData = reader.ReadBytes( havokDataSize );
            File.WriteAllBytes( HkxTempLocation, havokData );

            ModdedPapMod4 = ( int )( reader.BaseStream.Position % 4 );

            reader.BaseStream.Position = footerPosition;
            ModdedTmbOffset4 = ( int )( reader.BaseStream.Position % 4 );

            for( var i = 0; i < numAnimations; i++ ) {
                Animations[i].ReadTmb( reader );
                reader.ReadBytes( Padding( reader.BaseStream.Position, i, numAnimations, ModdedTmbOffset4 ) );
            }

            if( havokData.Length > 8 ) {
                MotionData = new( this, HkxTempLocation, init );
            }
            else {
                EmptyHavok = true;
            }

            if( verify ) Verified = FileUtils.Verify( reader, ToBytes() );
        }

        public override void Update() {
            base.Update();
            MotionData?.Write( Handles );
        }

        public override void Write( BinaryWriter writer ) {
            var tmbData = Animations.Select( x => x.GetTmbBytes() );

            var startPos = writer.BaseStream.Position;

            writer.Write( 0x20706170 );
            writer.Write( 0x00020001 );
            writer.Write( ( short )Animations.Count );
            ModelId.Write( writer );
            ModelType.Write( writer );
            Variant.Write( writer );

            var offsetPos = writer.BaseStream.Position; // coming back here later
            writer.Write( 0 ); // placeholders
            writer.Write( 0 );
            writer.Write( 0 );

            var infoPos = writer.BaseStream.Position;
            foreach( var anim in Animations ) anim.Write( writer );

            var havokPos = writer.BaseStream.Position;

            var havokData = File.ReadAllBytes( HkxTempLocation );
            writer.Write( havokData );

            FileUtils.PadTo( writer, writer.BaseStream.Position, 4, ModdedPapMod4 );

            var timelinePos = writer.BaseStream.Position;
            var idx = 0;
            foreach( var tmb in tmbData ) {
                writer.Write( tmb );
                FileUtils.Pad( writer, Padding( writer.BaseStream.Position, idx, tmbData.Count(), ModdedTmbOffset4 ) );
                idx++;
            }

            // go back and write sizes
            var endPos = writer.BaseStream.Position;
            writer.BaseStream.Position = offsetPos;
            writer.Write( ( int )( infoPos - startPos ) );
            writer.Write( ( int )( havokPos - startPos ) );
            writer.Write( ( int )( timelinePos - startPos ) );
            writer.BaseStream.Position = endPos;
        }

        public override void Draw() {
            using var _ = ImRaii.PushId( "Main" );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawAnimations();
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            ModelId.Draw();
            ModelType.Draw();
            Variant.Draw();

            if( ImGui.Button( $"Export Havok" ) ) {
                FileBrowserManager.SaveFileDialog( "Select a Save Location", ".hkx", "ExportedHavok", "hkx", ( bool ok, string res ) => {
                    if( ok ) File.Copy( HkxTempLocation, res, true );
                } );
            }

            ImGui.SameLine();

            MotionData?.DrawExportAll();
        }

        private void DrawAnimations() {
            using var tabItem = ImRaii.TabItem( "Animations" );
            if( !tabItem ) return;

            if( EmptyHavok ) {
                ImGui.TextDisabled( "No Havok data" );
                return;
            }

            AnimationsDropdown.Draw();
        }

        public override List<string> GetPapIds() => Animations.Select( x => x.GetName() ).ToList();

        public override List<short> GetPapTypes() => Animations.Select( x => x.GetPapType() ).ToList();

        public void RefreshHavokIndexes() {
            for( var i = 0; i < Animations.Count; i++ ) {
                Animations[i].HavokIndex = ( short )i;
            }
            AnimationsDropdown.ClearSelected();
        }

        private static int Padding( long position, int itemIdx, int numItems, int customOffset ) { // Don't pad the last element
            if( numItems > 1 && itemIdx < numItems - 1 ) {
                var leftOver = ( position - customOffset ) % 4;
                return ( int )( leftOver == 0 ? 0 : 4 - leftOver );
            }
            return 0;
        }

        public override void Dispose() {
            base.Dispose();
            MotionData?.Dispose();
            foreach( var item in Handles ) Marshal.FreeHGlobal( item );
            Handles.Clear();
        }
    }
}
