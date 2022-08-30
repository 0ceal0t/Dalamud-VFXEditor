using ImGuiNET;
using System.Diagnostics;
using VFXEditor.Utils;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.TMB {
    public class TMBNpcSelect : TMBSelectTab<XivNpc, XivNpcSelected> {
        public TMBNpcSelect( string parentId, string tabId, TMBSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Npcs, dialog ) {
        }

        protected override bool CheckMatch( XivNpc item, string searchInput ) {
            return Matches( item.Name, searchInput ) || Matches( item.Id, searchInput );
        }

        protected override void DrawExtra() => DrawThankYou();

        protected override void DrawSelected( XivNpcSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Npc.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Variant: " + loadedItem.Npc.Variant );

            DrawPath( "TMB", loadedItem.TmbPaths, Id, Dialog, SelectResultType.GameNpc, "NPC", loadedItem.Npc.Name );
        }

        protected override string UniqueRowTitle( XivNpc item ) {
            return item.Name + Id + item.RowId;
        }
    }
}