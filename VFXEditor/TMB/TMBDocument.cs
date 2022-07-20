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

        protected override void UpdateFile() {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote TMB file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override void Update() {
            UpdateFile();
            Reload();
            Plugin.ResourceLoader.ReRender();
        }

        protected override void ExportRaw() {
            UIHelper.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );
        }

        protected override bool GetVerified() => CurrentFile.Verified;

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DisplayAnimationWarning();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( "##Tmb" );
            }
        }

        protected override void SourceShow() => TMBManager.SourceSelect.Show();

        protected override void ReplaceShow() => TMBManager.ReplaceSelect.Show();
    }
}
