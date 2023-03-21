using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;

namespace VfxEditor.EidFormat {
    /*public class EidDocument : FileManagerDocument<EidFile, WorkspaceMetaBasic> {
        public EidDocument( string writeLocation ) : base( writeLocation, "Eid" ) { }
        public EidDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Eid" ) { }

        protected override string GetExtensionWithoutDot() => "eid";

        protected override EidFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        public override void CheckKeybinds() {
            //if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Eid.Copy();
            //if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Eid.Paste();
            //if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Eid?.Undo();
            //if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Eid?.Redo();
        }

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( "##Eid" );
            }
        }

        protected override void ReplaceShow() => EidManager.SourceSelect.Show();

        protected override void SourceShow() => EidManager.ReplaceSelect.Show();

        protected override bool ExtraInputColumn() => false;
    }*/
}
