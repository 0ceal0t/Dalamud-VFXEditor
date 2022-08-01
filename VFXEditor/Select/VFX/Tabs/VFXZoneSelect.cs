using ImGuiNET;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.VFX {
    public class VFXZoneSelect : VFXSelectTab<XivZone, XivZoneSelected> {
        public VFXZoneSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Zones, dialog ) {
        }

        protected override bool CheckMatch( XivZone item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void DrawSelected( XivZoneSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Zone.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "LGB Path: " );
            ImGui.SameLine();
            DisplayPath( loadedItem.Zone.LgbPath );

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameZone, "ZONE", loadedItem.Zone.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivZone item ) {
            return item.Name + Id + item.RowId;
        }
    }
}