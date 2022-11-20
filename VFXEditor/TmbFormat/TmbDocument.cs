using ImGuiNET;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.Animation;
using VfxEditor.Data;
using VfxEditor.FileManager;

namespace VfxEditor.TmbFormat {
    public partial class TmbDocument : FileManagerDocument<TmbFile, WorkspaceMetaTmb> {
        public uint AnimationId = 0;
        private bool AnimationDisabled => string.IsNullOrEmpty( ReplacePath ) || AnimationId == 0;

        public TmbDocument( string writeLocation ) : base( writeLocation, "Tmb" ) { }
        public TmbDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace ) : base( writeLocation, localPath, source, replace, "Tmb" ) {
            AnimationId = ActorAnimationManager.GetIdFromTmbPath( ReplacePath );
        }

        public override void Update() {
            UpdateFile();
            Reload();
            Plugin.ResourceLoader.ReRender();
        }

        protected override string GetExtensionWithoutDot() => "tmb";

        protected override TmbFile FileFromReader( BinaryReader reader ) => new( reader, false );

        public override void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) CopyManager.Tmb.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) CopyManager.Tmb.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) CommandManager.Tmb?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) CommandManager.Tmb?.Redo();
        }

        public override WorkspaceMetaTmb GetWorkspaceMeta( string newPath ) => new() {
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        protected override void SourceShow() => TmbManager.SourceSelect.Show();

        protected override void ReplaceShow() => TmbManager.ReplaceSelect.Show();

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
