using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdOrchestrionSelect : ScdSelectTab<XivOrchestrion, XivOrchestrionSelected> {
        public ScdOrchestrionSelect( string parentId, string tabId, ScdSelectDialog dialog ) : base( parentId, tabId, SheetManager.Orchestrions, dialog ) { }

        protected override bool CheckMatch( XivOrchestrion item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void DrawSelected( XivOrchestrionSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Orchestrion.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawPath( "Path", loadedItem.Path, Id, Dialog, SelectResultType.GameMusic, "MUSIC", loadedItem.Orchestrion.Name );
        }

        protected override string UniqueRowTitle( XivOrchestrion item ) => $"{item.Name}##{item.RowId}";
    }
}