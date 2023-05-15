using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
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

        public readonly ParsedShort ModelId = new( "Model Id" );
        public readonly ParsedEnum<SkeletonType> ModelType = new( "Skeleton Type", size: 1 );
        public readonly ParsedInt Variant = new( "Variant", size: 1 );

        public readonly List<PapAnimation> Animations = new();
        public readonly PapAnimationDropdown AnimationsDropdown;

        // Pap files from mods sometimes get exported with a weird padding, so we have to account for that
        private readonly int ModdedTmbOffset4 = 0;
        private readonly int ModdedPapMod4 = 0;

        public PapFile( BinaryReader reader, string hkxTemp, bool checkOriginal = true ) : base( new( Plugin.PapManager ) ) {
            AnimationsDropdown = new( this, Animations );
            HkxTempLocation = hkxTemp;

            var startPos = reader.BaseStream.Position;

            byte[] original = null;
            if( checkOriginal ) {
                original = FileUtils.ReadAllBytes( reader );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

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
                Animations.Add( new PapAnimation( reader, HkxTempLocation ) );
            }

            // ... do something about havok data ...
            var havokDataSize = footerPosition - havokPosition;
            reader.BaseStream.Seek( havokPosition, SeekOrigin.Begin );
            var havokData = reader.ReadBytes( havokDataSize );
            File.WriteAllBytes( HkxTempLocation, havokData );

            ModdedPapMod4 = ( int )( reader.BaseStream.Position % 4 );

            reader.BaseStream.Seek( footerPosition, SeekOrigin.Begin );
            ModdedTmbOffset4 = ( int )( reader.BaseStream.Position % 4 );

            for( var i = 0; i < numAnimations; i++ ) {
                Animations[i].ReadTmb( reader, Command );
                reader.ReadBytes( Padding( reader.BaseStream.Position, i, numAnimations, ModdedTmbOffset4 ) );
            }

            if( checkOriginal ) { // Check if output matches the original
                Verified = FileUtils.CompareFiles( original, ToBytes(), out var _ );
            }
        }

        public override void Write( BinaryWriter writer ) {
            var havokData = File.ReadAllBytes( HkxTempLocation );
            var tmbData = Animations.Select( x => x.GetTmbBytes() );

            writer.BaseStream.Seek( 0, SeekOrigin.Begin );

            var startPos = writer.BaseStream.Position;

            writer.Write( 0x20706170 );
            writer.Write( 0x00020001 );
            writer.Write( ( short )Animations.Count );
            ModelId.Write( writer );
            ModelType.Write( writer );
            Variant.Write( writer );

            var offsetPos = writer.BaseStream.Position; // coming back here later
            writer.Write( 0 ); // placeholders, will come back later
            writer.Write( 0 );
            writer.Write( 0 );

            var infoPos = writer.BaseStream.Position;
            foreach( var anim in Animations ) anim.Write( writer );

            var havokPos = writer.BaseStream.Position;
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
            writer.BaseStream.Seek( offsetPos, SeekOrigin.Begin );
            writer.Write( ( int )( infoPos - startPos ) );
            writer.Write( ( int )( havokPos - startPos ) );
            writer.Write( ( int )( timelinePos - startPos ) );
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

            ModelId.Draw( CommandManager.Pap );
            ModelType.Draw( CommandManager.Pap );
            Variant.Draw( CommandManager.Pap );

            if( ImGui.Button( $"Export all Havok data" ) ) {
                FileDialogManager.SaveFileDialog( "Select a Save Location", ".hkx", "", "hkx", ( bool ok, string res ) => {
                    if( ok ) File.Copy( HkxTempLocation, res, true );
                } );
            }
        }

        private void DrawAnimations() {
            using var tabItem = ImRaii.TabItem( "Animations" );
            if( !tabItem ) return;

            AnimationsDropdown.Draw();
        }

        public List<string> GetPapIds() => Animations.Select( x => x.GetName() ).ToList();

        public void RefreshHavokIndexes() {
            for( var i = 0; i < Animations.Count; i++ ) {
                Animations[i].HavokIndex = ( short )i;
            }
        }

        private static int Padding( long position, int itemIdx, int numItems, int customOffset ) { // Don't pad the last element
            if( numItems > 1 && itemIdx < numItems - 1 ) {
                var leftOver = ( position - customOffset ) % 4;
                return ( int )( leftOver == 0 ? 0 : 4 - leftOver );
            }
            return 0;
        }
    }
}
