using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxCutsceneSelect : VfxSelectTab<XivCutscene, XivCutsceneSelected> {
        public VfxCutsceneSelect( string parentId, string tabId, VfxSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Cutscenes, dialog ) {
        }

        protected override bool CheckMatch( XivCutscene item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void DrawSelected( XivCutsceneSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Cutscene.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "CUTB Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.Cutscene.Path );

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameCutscene, "CUT", loadedItem.Cutscene.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivCutscene item ) => $"{item.Name}{Id}{item.RowId}";
    }
}