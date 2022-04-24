using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.VFX {
    public class VFXCutsceneSelect : VFXSelectTab<XivCutscene, XivCutsceneSelected> {
        public VFXCutsceneSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Cutscenes, dialog ) {
        }

        protected override bool CheckMatch( XivCutscene item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void DrawSelected( XivCutsceneSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Cutscene.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "CUTB Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.Cutscene.Path );

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameCutscene, "CUT", loadedItem.Cutscene.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivCutscene item ) {
            return item.Name + Id + item.RowId;
        }
    }
}