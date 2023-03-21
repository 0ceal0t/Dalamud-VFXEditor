using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdDocument : FileManagerDocument<ScdFile, WorkspaceMetaBasic> {
        public ScdDocument( ScdManager manager, string writeLocation ) : base( manager, writeLocation, "Scd", "scd" ) { }
        public ScdDocument( ScdManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) : 
            base( manager, writeLocation, localPath, source, replace, "Scd", "scd" ) { }

        protected override ScdFile FileFromReader( BinaryReader reader ) => new( reader );

        public override void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Scd.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Scd.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Scd?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Scd?.Redo();
        }

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
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

        protected override bool ExtraInputColumn() => false;
    }
}
