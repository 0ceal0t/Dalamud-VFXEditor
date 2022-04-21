using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.PAP {
    public partial class PAPDocument : FileManagerDocument<PAPFile, WorkspaceMetaPap> {
        private string HkxTemp => WriteLocation.Replace( ".pap", "_temp.hkx" );

        public PAPDocument( string writeLocation ) : base( writeLocation, "Pap", "PAP" ) {
        }
        public PAPDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Pap", "PAP" ) {
        }

        protected override void LoadLocal( string localPath ) {
            if (File.Exists(localPath)) {
                try {
                    using BinaryReader br = new( File.Open( localPath, FileMode.Open ) );
                    CurrentFile = new( br, HkxTemp );
                    UIHelper.OkNotification( "PAP file loaded" );
                }
                catch(Exception e) {
                    PluginLog.Error( "Error Reading File", e );
                    UIHelper.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void LoadGame( string gamePath ) {
            if( Plugin.DataManager.FileExists( gamePath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile( gamePath );
                    using var ms = new MemoryStream( file.Data );
                    using var br = new BinaryReader( ms );
                    CurrentFile = new PAPFile( br, HkxTemp );
                    UIHelper.OkNotification( "PAP file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error Reading File", e );
                    UIHelper.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void Update() {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote PAP file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override bool GetVerified() => CurrentFile.Verified;

        protected override void DrawBody() {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.9f, 0.1f, 0.1f, 1.0f ) );
            ImGui.TextWrapped( "DO NOT modify movement abilities (dashes, backflips, etc.)" );
            ImGui.PopStyleColor();

            ImGui.TextWrapped( "Please read a guide before attempting to modify a .tmb or .pap file" );
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.SmallButton( "Guides##Pap" ) ) UIHelper.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) {
                DisplayBeginHelpText();
            }
            else {
                if( UIHelper.OkButton( "UPDATE" ) ) {
                    Update();
                    Reload( CurrentFile.GetPapIds() );
                    Plugin.ResourceLoader.ReRender();
                }

                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.PopFont();
                    UIHelper.WriteBytesDialog( ".pap", CurrentFile.ToBytes(), "pap" );
                }
                else ImGui.PopFont();

                ImGui.SameLine();
                UIHelper.ShowVerifiedStatus( Verified );

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                CurrentFile.Draw( "##Pap" );
            }
        }

        protected override void SourceShow() => PAPManager.SourceSelect.Show();

        protected override void ReplaceShow() => PAPManager.ReplaceSelect.Show();

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }
    }
}
