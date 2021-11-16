using Dalamud.Interface;
using ImGuiNET;
using System.IO;
using System.Numerics;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.Pap {
    public partial class PapDocument : FileManagerDocument<PapFile, WorkspaceMetaPap> {
        private string HkxTemp => WriteLocation.Replace( ".pap", "_temp.hkx" );

        public PapDocument( string writeLocation ) : base(writeLocation, "Pap", "PAP") {
        }
        public PapDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base(writeLocation, localPath, source, replace, "Pap", "PAP") {
        }

        protected override void LoadLocal( string localPath ) {
            if( !File.Exists( localPath ) ) return;
            using BinaryReader br = new( File.Open( localPath, FileMode.Open ) );
            CurrentFile = new( br, HkxTemp );
        }

        protected override void LoadGame( string gamePath ) {
            if( !Plugin.DataManager.FileExists( gamePath ) ) return;
            var file = Plugin.DataManager.GetFile( gamePath );
            using var ms = new MemoryStream( file.Data );
            using var br = new BinaryReader( ms );
            CurrentFile = new PapFile( br, HkxTemp );
        }

        protected override void Update() {
            if( CurrentFile == null ) return;
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override bool GetVerified() => true;

        protected override void DrawBody() {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.9f, 0.1f, 0.1f, 1.0f ) );
            ImGui.TextWrapped( "DO NOT modify movement abilities (dashes, backflips, etc.)" );
            ImGui.PopStyleColor();

            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile != null ) {
                if( UiHelper.OkButton( "UPDATE" ) ) Update();

                ImGui.SameLine();
                if( ImGui.Button( "Reload" ) ) Reload();
                ImGui.SameLine();
                UiHelper.HelpMarker( "Manually reload the resource" );

                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.PopFont();
                    Plugin.WriteBytesDialog( ".pap", CurrentFile.ToBytes(), "pap" );
                }
                else ImGui.PopFont();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                CurrentFile.Draw( "##Pap" );
            }
        }

        protected override void SourceShow() => PapManager.SourceSelect.Show();

        protected override void ReplaceShow() => PapManager.ReplaceSelect.Show();

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }
    }
}
