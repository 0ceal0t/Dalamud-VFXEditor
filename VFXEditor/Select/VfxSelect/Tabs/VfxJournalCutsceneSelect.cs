using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxJournalCutsceneSelect : SelectTab<XivJournalCutscene, XivJournalCutsceneSelected> {
        public VfxJournalCutsceneSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.JournalCutscenes, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            for( var idx = 0; idx < Loaded.VfxPaths.Count; idx++ ) {
                var paths = Loaded.VfxPaths[idx];
                if( ImGui.CollapsingHeader( $"Cutscene {idx}{parentId}") ) {
                    ImGui.Indent();
                    DrawPath( "VFX", paths, $"{parentId}{idx}", SelectResultType.GameCutscene, $"{Loaded.Cutscene.Name} {idx}", true );
                    ImGui.Unindent();
                }
            }
        }

        protected override string GetName( XivJournalCutscene item ) => item.Name;
    }
}