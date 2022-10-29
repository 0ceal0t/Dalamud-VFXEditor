using ImGuiNET;
using System.Diagnostics;
using VfxEditor.Utils;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.PapSelect {
    public class PapNpcSelect : PapSelectTab<XivNpc, XivNpcSelected> {
        public PapNpcSelect( string parentId, string tabId, PapSelectDialog dialog ) :
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

            DrawPath( "PAP", loadedItem.PapPaths, Id, Dialog, SelectResultType.GameNpc, "NPC", loadedItem.Npc.Name );
        }

        protected override string UniqueRowTitle( XivNpc item ) {
            return item.Name + Id + item.RowId;
        }
    }
}