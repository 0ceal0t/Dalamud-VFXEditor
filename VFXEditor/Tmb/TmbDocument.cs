using Dalamud.Interface;
using ImGuiNET;
using System.IO;
using System.Numerics;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.Tmb {
    public partial class TmbDocument : FileManagerDocument<TmbFile, WorkspaceMetaTmb> {
        public TmbDocument( string writeLocation ) : base(writeLocation, "Tmb", "TMB") {
        }
        public TmbDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base(writeLocation, localPath, source, replace, "Tmb", "TMB") {
        }

        protected override void LoadLocal( string localPath ) {
            if( !File.Exists( localPath ) ) return;
            using BinaryReader br = new( File.Open( localPath, FileMode.Open ) );
            CurrentFile = new( br );
        }

        protected override void LoadGame( string gamePath ) {
            if( !Plugin.DataManager.FileExists( gamePath ) ) return;
            var file = Plugin.DataManager.GetFile( gamePath );
            using var ms = new MemoryStream( file.Data );
            using var br = new BinaryReader( ms );
            CurrentFile = new TmbFile( br );
        }

        protected override void Update() {
            if( CurrentFile == null ) return;
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override bool GetVerified() => CurrentFile.Verified;

        protected override void DrawBody() {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.9f, 0.1f, 0.1f, 1.0f ) );
            ImGui.TextWrapped( "DO NOT modify movement abilities (dashes, backflips, etc.)" );
            ImGui.PopStyleColor();

            ImGui.TextWrapped( "Also note that changing animation paths may not work without swapping the .pap files as well" );

            ImGui.TextWrapped( "When replacing a .pap or .tmb, you may need to change the animation name (such as cbbm_ws01). Make sure to do this in the .tmb and BOTH places in the .pap file. If you have questions, please ask them in the Quicklauncher discord or check the guides" );
            if( ImGui.Button( "Guides##Pap" ) ) Plugin.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

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
                    Plugin.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );
                }
                else ImGui.PopFont();

                ImGui.SameLine();
                UiHelper.ShowVerifiedStatus( Verified );

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                CurrentFile.Draw( "##Tmb" );
            }
        }

        protected override void SourceShow() => TmbManager.SourceSelect.Show();

        protected override void ReplaceShow() => TmbManager.ReplaceSelect.Show();
    }
}
