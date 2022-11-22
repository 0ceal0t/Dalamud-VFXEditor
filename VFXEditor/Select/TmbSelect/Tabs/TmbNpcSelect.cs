using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbNpcSelect : TmbSelectTab<XivNpc, XivNpcSelected> {
        public TmbNpcSelect( string parentId, string tabId, TmbSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Npcs, dialog ) {
        }

        protected override bool CheckMatch( XivNpc item, string searchInput ) => Matches( item.Name, searchInput ) || Matches( item.Id, searchInput );

        protected override void DrawExtra() => DrawThankYou();

        protected override void DrawSelected( XivNpcSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Npc.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Variant: " + loadedItem.Npc.Variant );

            DrawPath( "TMB", loadedItem.TmbPaths, Id, Dialog, SelectResultType.GameNpc, "NPC", loadedItem.Npc.Name, true );
        }

        protected override string UniqueRowTitle( XivNpc item )  => $"{item.Name}{Id}{item.RowId}";
    }
}