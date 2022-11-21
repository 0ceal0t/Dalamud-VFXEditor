using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using VfxEditor.FileManager;

namespace VfxEditor.ScdFormat {
    public partial class ScdDocument : FileManagerDocument<ScdFile, WorkspaceMetaScd> {
        public ScdDocument( string writeLocation ) : base( writeLocation, "Scd" ) { }
        public ScdDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Scd" ) { }

        public override void Update() {
            UpdateFile();
            Reload();
            Plugin.ResourceLoader.ReRender();
        }

        protected override string GetExtensionWithoutDot() => "scd";

        protected override ScdFile FileFromReader( BinaryReader reader ) {
            return new( reader );
        }

        public override void CheckKeybinds() { }

        public override WorkspaceMetaScd GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( "##Scd" );
            }
        }

        protected override void SourceShow() => ScdManager.SourceSelect.Show();

        protected override void ReplaceShow() => ScdManager.ReplaceSelect.Show();

        protected override bool ExtraInputColumn() => false;
    }
}
