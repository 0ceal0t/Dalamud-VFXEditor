using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.IO;
using System.Numerics;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.Tmb {
    public partial class TmbDocument : FileManagerDocument<TmbFile, WorkspaceMetaTmb> {
        public TmbDocument( string writeLocation ) : base( writeLocation, "Tmb", "TMB" ) {
        }
        public TmbDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Tmb", "TMB" ) {
        }

        protected override void LoadLocal( string localPath ) {
            CurrentFile = TmbFile.FromLocalFile( localPath );
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
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote TMB file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override bool GetVerified() => CurrentFile.Verified;

        protected override void DrawBody() {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.9f, 0.1f, 0.1f, 1.0f ) );
            ImGui.TextWrapped( "DO NOT modify movement abilities (dashes, backflips, etc.)" );
            ImGui.PopStyleColor();

            ImGui.TextWrapped( "Please read a guide before attempting to modify a .tmb or .pap file: " );
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.SmallButton( "Guides##Pap" ) ) UiHelper.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) {
                DisplayBeginHelpText();
            }
            else {
                if( UiHelper.OkButton( "UPDATE" ) ) {
                    Update();
                    Reload();
                    Plugin.ResourceLoader.ReRender();
                }

                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.PopFont();
                    UiHelper.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );
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
