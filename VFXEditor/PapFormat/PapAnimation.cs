using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;
using VfxEditor.TmbFormat;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapAnimation {
        public readonly PapFile File;

        public short HavokIndex = 0;
        public readonly string HkxTempLocation;

        private readonly ParsedPaddedString Name = new( "Name", "cbbm_replace_this", 32, 0x00 );
        private readonly ParsedShort Type = new( "Type" );
        private readonly ParsedBool Face = new( "Face Animation" );
        public TmbFile Tmb;

        //public readonly SkeletonView Skeleton;

        public PapAnimation( PapFile file, string hkxPath ) {
            File = file;
            HkxTempLocation = hkxPath;
            //Skeleton = new( File, this );
        }

        public PapAnimation( PapFile file, BinaryReader reader, string hkxPath ) {
            File = file;
            HkxTempLocation = hkxPath;

            Name.Read( reader );
            Type.Read( reader );
            HavokIndex = reader.ReadInt16();
            Face.Read( reader );

            //Skeleton = new( File, this );
        }

        public void Write( BinaryWriter writer ) {
            Name.Write( writer );
            Type.Write( writer );
            writer.Write( HavokIndex );
            Face.Write( writer );
        }

        public void ReadTmb( BinaryReader reader, CommandManager manager ) {
            Tmb = new TmbFile( reader, manager, false );
        }

        public void ReadTmb( string path, CommandManager manager ) {
            Tmb = TmbFile.FromPapEmbedded( path, manager );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name.Value;

        public void Draw() {
            SheetData.InitMotionTimelines();
            if( !string.IsNullOrEmpty( Name.Value ) && SheetData.MotionTimelines.TryGetValue( Name.Value, out var motionData ) ) {
                ImGui.TextDisabled( $"Loop: [{motionData.Loop}] Lip: [{motionData.Lip}] Blink: [{motionData.Blink}]" );

                ImGui.SameLine();
                UiUtils.HelpMarker( "These values are hard-coded in the game's MotionTimeline sheet, and are based on the animation name" );
            }

            Name.Draw( File.Command );
            Type.Draw( File.Command );
            Face.Draw( File.Command );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.TextDisabled( $"This animation has Havok index: {HavokIndex}" );

            if( ImGui.Button( $"Replace Havok" ) ) {
                FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                    if( ok ) Plugin.AddModal( new PapIndexModal( this, res ) );
                } );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "AnimationTabs" );
            if( !tabBar ) return;

            DrawTmb();
            DrawAnimation3D();
        }

        private void DrawTmb() {
            using var tabItem = ImRaii.TabItem( "TMB" );
            if( !tabItem ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                if( ImGui.Button( "Export" ) ) UiUtils.WriteBytesDialog( ".tmb", Tmb.ToBytes(), "tmb" );

                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) {
                    FileDialogManager.OpenFileDialog( "Select a File", ".tmb,.*", ( bool ok, string res ) => {
                        if( ok ) {
                            File.Command.Add( new PapReplaceTmbCommand( this, TmbFile.FromPapEmbedded( res, File.Command ) ) );
                            UiUtils.OkNotification( "TMB data imported" );
                        }
                    } );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var _ = ImRaii.PushId( "Tmb" );
            Tmb.Draw();
        }

        private void DrawAnimation3D() {
            using var tabItem = ImRaii.TabItem( "3D View" );
            if( !tabItem ) return;

            File.AnimationData.Draw( HavokIndex );
            //Skeleton.Draw();
        }
    }
}
