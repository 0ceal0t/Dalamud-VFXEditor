using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public partial class PapDocument : FileManagerDocument<PapFile, WorkspaceMetaBasic> {
        private string HkxTemp => WriteLocation.Replace( ".pap", "_temp.hkx" );

        public PapDocument( PapManager manager, string writeLocation ) : base( manager, writeLocation, "Pap", "pap" ) { }
        public PapDocument( PapManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) : 
            base( manager, writeLocation, localPath, source, replace, "Pap", "pap" ) { }

        // Need to pass PapIds
        public override void Update() {
            UpdateFile();
            Reload( CurrentFile.GetPapIds() );
            Plugin.ResourceLoader.ReRender();
        }

        protected override PapFile FileFromReader( BinaryReader reader ) => new( reader, HkxTemp );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        public override void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Pap.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Pap.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Pap?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Pap?.Redo();
        }

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

        protected override bool ExtraInputColumn() => false;

        public override void Dispose() {
            base.Dispose();
            File.Delete( HkxTemp );
        }
    }
}
