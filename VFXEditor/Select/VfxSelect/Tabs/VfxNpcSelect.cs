using ImGuiNET;
using System.Diagnostics;
using VfxEditor.Utils;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxNpcSelect : VfxSelectTab<XivNpc, XivNpcSelected> {
        public VfxNpcSelect( string parentId, string tabId, VfxSelectDialog dialog ) :
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

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameNpc, "NPC", loadedItem.Npc.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivNpc item ) {
            return item.Name + Id + item.RowId;
        }
    }
}