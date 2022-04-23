using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VFXEditor.FileManager;
using VFXEditor.Helper;
using VFXSelect;

namespace VFXEditor.TMB {
    public partial class TMBDocument : FileManagerDocument<TMBFile, WorkspaceMetaTmb> {
        public TMBDocument( string writeLocation ) : base( writeLocation, "Tmb", "TMB" ) {
        }
        public TMBDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Tmb", "TMB" ) {
        }

        protected override void LoadLocal( string localPath ) {
            if( File.Exists( localPath ) ) {
                try {
                    CurrentFile = TMBFile.FromLocalFile( localPath );
                    UIHelper.OkNotification( "TMB file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error Reading File", e );
                    PluginLog.Error( e.ToString() );
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
                    CurrentFile = new TMBFile( br );
                    UIHelper.OkNotification( "TMB file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error Reading File", e );
                    PluginLog.Error( e.ToString() );
                    UIHelper.ErrorNotification( "Error reading file" );
                }
            }
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
            if( ImGui.SmallButton( "Guides##Pap" ) ) UIHelper.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) {
                DisplayBeginHelpText();
            }
            else {
                if( UIHelper.OkButton( "UPDATE" ) ) {
                    Update();
                    Reload();
                    Plugin.ResourceLoader.ReRender();
                }

                ImGui.SameLine();
                ImGui.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) {
                    ImGui.PopFont();
                    UIHelper.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );
                }
                else ImGui.PopFont();

                ImGui.SameLine();
                UIHelper.ShowVerifiedStatus( Verified );

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                CurrentFile.Draw( "##Tmb" );
            }
        }

        protected override void SourceShow() => TMBManager.SourceSelect.Show();

        protected override void ReplaceShow() => TMBManager.ReplaceSelect.Show();
    }
}
