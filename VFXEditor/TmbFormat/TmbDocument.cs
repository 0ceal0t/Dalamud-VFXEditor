using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.Animation;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbDocument : FileManagerDocument<TmbFile, WorkspaceMetaBasic> {
        public uint AnimationId = 0;
        private bool AnimationDisabled => string.IsNullOrEmpty( ReplacePath ) || AnimationId == 0;

        public TmbDocument( TmbManager manager, string writeLocation ) : base( manager, writeLocation, "Tmb", "tmb" ) { }
        public TmbDocument( TmbManager manager, string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( manager, 
            writeLocation, localPath, source, replace, "Tmb", "tmb" ) {
            AnimationId = ActorAnimationManager.GetIdFromTmbPath( ReplacePath );
        }

        protected override TmbFile FileFromReader( BinaryReader reader ) => new( reader, false );

        public override void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Tmb.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Tmb.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Tmb?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Tmb?.Redo();
        }

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        protected override bool ExtraInputColumn() => true;

        protected override void DrawSearchBarsColumn() {
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 245 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );
            DisplaySearchBars();
            ImGui.PopItemWidth();
        }

        protected override void DrawExtraColumn() {
            ImGui.SetColumnWidth( 3, 120 );

            if( AnimationDisabled ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, 0.5f );
            if( ImGui.Button( "Play", new Vector2( 60, 23 ) ) && !AnimationDisabled ) Plugin.ActorAnimationManager.Apply( AnimationId );
            if( AnimationDisabled ) ImGui.PopStyleVar();

            if( ImGui.Button( "Reset", new Vector2( 60, 23 ) ) ) Plugin.ActorAnimationManager.Reset();
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
                CurrentFile.Draw( "##Tmb" );
            }
        }
    }
}
