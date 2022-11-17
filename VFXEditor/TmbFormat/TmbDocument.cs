using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public partial class TmbDocument : FileManagerDocument<TmbFile, WorkspaceMetaTmb> {
        public TmbDocument( string writeLocation ) : base( writeLocation, "Tmb", "TMB" ) { }
        public TmbDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Tmb", "TMB" ) { }

        public override void Update() {
            UpdateFile();
            Reload();
            Plugin.ResourceLoader.ReRender();
        }

        public override void CheckKeybinds() { }

        protected override void LoadLocal( string localPath ) {
            if( File.Exists( localPath ) ) {
                try {
                    CurrentFile = TmbFile.FromLocalFile( localPath );
                    UiUtils.OkNotification( "TMB file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error Reading File", e );
                    UiUtils.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void LoadGame( string gamePath ) {
            if( Plugin.DataManager.FileExists( gamePath ) ) {
                try {
                    var file = Plugin.DataManager.GetFile( gamePath );
                    using var ms = new MemoryStream( file.Data );
                    using var br = new BinaryReader( ms );
                    CurrentFile = new TmbFile( br );
                    UiUtils.OkNotification( "TMB file loaded" );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error Reading File" );
                    UiUtils.ErrorNotification( "Error reading file" );
                }
            }
        }

        protected override void UpdateFile() {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote TMB file to {0}", WriteLocation );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected override void ExportRaw() => UiUtils.WriteBytesDialog( ".tmb", CurrentFile.ToBytes(), "tmb" );

        protected override bool IsVerified() => CurrentFile.IsVerified;

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

        protected override void SourceShow() => TmbManager.SourceSelect.Show();

        protected override void ReplaceShow() => TmbManager.ReplaceSelect.Show();
    }
}
