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
            if( File.Exists( localPath ) ) {
                try {
                    using BinaryReader br = new( File.Open( localPath, FileMode.Open ) );
                    CurrentFile = new( br, HkxTemp );
                    UIHelper.OkNotification( "PAP file loaded" );
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
                    CurrentFile = new PAPFile( br, HkxTemp );
                    UIHelper.OkNotification( "PAP file loaded" );
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
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote PAP file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override void Update() {
            UpdateFile();
            Reload( CurrentFile.GetPapIds() );
            Plugin.ResourceLoader.ReRender();
        }

        protected override void ExportRaw() {
            UIHelper.WriteBytesDialog( ".pap", CurrentFile.ToBytes(), "pap" );
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
