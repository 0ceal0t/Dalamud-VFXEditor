using Dalamud.Interface;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.FileBrowser;
using VfxEditor.Parsing;
using VfxEditor.Parsing.String;
using VfxEditor.TmbFormat;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapAnimation {
        private readonly List<string> Prefixes = new() {
            "cbfp",
            "cbfa",
            "cbep",
            "cbba",
            "csnw",
            "cbbm",
            "cbbp",
            "csxm",
            "cblw",
            "csbw",
            "cblm",
            "cbem",
            "cbna",
            "cfxf",
            "pc",
            "cbnm",
            "cbew",
            "cbbw",
            "f",
            "cbnw",
            "cbnp",
            "cfxl",
            "cbfm",
            "cbfw",
            "cfxb",
        };

        public readonly PapFile File;

        public short HavokIndex = 0;
        public readonly string HkxTempLocation;

        private readonly ParsedPaddedString Name = new( "Name", "cbbm_replace_this", 32, 0x00 );
        private readonly ParsedShort Type = new( "Type" );
        private readonly ParsedBool Face = new( "Face Animation" );
        public TmbFile Tmb;

        public PapAnimation( PapFile file, string hkxPath ) {
            File = file;
            HkxTempLocation = hkxPath;
        }

        public PapAnimation( PapFile file, BinaryReader reader, string hkxPath ) {
            File = file;
            HkxTempLocation = hkxPath;

            Name.Read( reader );
            Type.Read( reader );
            HavokIndex = reader.ReadInt16();
            Face.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Name.Write( writer );
            Type.Write( writer );
            writer.Write( HavokIndex );
            Face.Write( writer );
        }

        public void ReadTmb( BinaryReader reader ) {
            Tmb = new TmbFile( reader, File.Command, false );
        }

        public void ReadTmb( string path ) {
            Tmb = TmbFile.FromPapEmbedded( path, File.Command );
        }

        public byte[] GetTmbBytes() => Tmb.ToBytes();

        public string GetName() => Name.Value;

        public short GetPapType() => ( short )Type.Value;

        public void Draw() {
            SheetData.InitMotionTimelines();
            if( !string.IsNullOrEmpty( Name.Value ) && SheetData.MotionTimelines.TryGetValue( Name.Value, out var motionData ) ) {
                UiUtils.DrawIntText( "Blend Group:", motionData.Group );
                ImGui.SameLine();
                UiUtils.DrawBoolText( "Loop:", motionData.Loop );
                ImGui.SameLine();
                UiUtils.DrawBoolText( "Lips:", motionData.Lip );
                ImGui.SameLine();
                UiUtils.DrawBoolText( "Blink:", motionData.Blink );
                ImGui.SameLine();
                UiUtils.HelpMarker( "These values are hard-coded in the game's MotionTimeline sheet, and are based on the animation name" );
            }

            Name.Draw();
            if( string.IsNullOrEmpty( Name.Value ) || !Name.Value.Contains( '_' ) || !Prefixes.Contains( Name.Value.Split( "_" )[0] ) ) {
                using var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.RED_COLOR );
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( FontAwesomeIcon.InfoCircle.ToIconString() );
                }
                ImGui.SameLine();
                ImGui.TextWrapped( "Animation name must start with a valid prefix, such as cbbm_" );
            }

            Type.Draw();
            Face.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.TextDisabled( $"This animation has Havok index: {HavokIndex}" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using var tabBar = ImRaii.TabBar( "AnimationTabs" );
            if( !tabBar ) return;

            DrawTmb();
            DrawHavok();
            DrawMotion();
        }

        private void DrawTmb() {
            using var tabItem = ImRaii.TabItem( "TMB" );
            if( !tabItem ) return;

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                if( ImGui.Button( "Export" ) ) UiUtils.WriteBytesDialog( ".tmb", Tmb.ToBytes(), "tmb", "ExportedTmb" );

                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) {
                    FileBrowserManager.OpenFileDialog( "Select a File", ".tmb,.*", ( bool ok, string res ) => {
                        if( ok ) {
                            CommandManager.Add( new PapReplaceTmbCommand( this, TmbFile.FromPapEmbedded( res, File.Command ) ) );
                            UiUtils.OkNotification( "Tmb data imported" );
                        }
                    } );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var _ = ImRaii.PushId( "Tmb" );
            Tmb.Draw();
        }

        private void DrawMotion() {
            using var tabItem = ImRaii.TabItem( "Motion" );
            if( !tabItem ) return;

            File.MotionData.Draw( HavokIndex );
        }

        private void DrawHavok() {
            using var tabItem = ImRaii.TabItem( "Havok" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Havok" );

            File.MotionData.DrawHavok( HavokIndex );
        }
    }
}
