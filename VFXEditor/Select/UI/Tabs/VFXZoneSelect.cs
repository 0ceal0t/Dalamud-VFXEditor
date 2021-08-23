using ImGuiNET;
using VFXSelect.Data.Rows;

namespace VFXSelect.UI {
    public class VFXZoneSelect : VFXSelectTab<XivZone, XivZoneSelected> {
        public VFXZoneSelect( string parentId, string tabId, VFXSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Zones, dialog ) {
        }

        public override bool CheckMatch( XivZone item, string searchInput ) {
            return VFXSelectDialog.Matches( item.Name, searchInput );
        }

        public override void DrawSelected( XivZoneSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Zone.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "LGB Path: " );
            ImGui.SameLine();
            VFXSelectDialog.DisplayPath( loadedItem.Zone.LgbPath );
            var vfxIdx = 0;
            foreach( var path in loadedItem.VfxPaths ) {
                ImGui.Text( "VFX #" + vfxIdx + ": " );
                ImGui.SameLine();
                VFXSelectDialog.DisplayPath( path );
                if( ImGui.Button( "SELECT" + Id + vfxIdx ) ) {
                    Dialog.Invoke( new VFXSelectResult( VFXSelectType.GameZone, "[ZONE] " + loadedItem.Zone.Name + " #" + vfxIdx, path ) );
                }
                ImGui.SameLine();
                VFXSelectDialog.Copy( path, id: Id + "Copy" + vfxIdx );
                Dialog.Spawn( path, id: Id + "Spawn" + vfxIdx );
                vfxIdx++;
            }
        }

        public override string UniqueRowTitle( XivZone item ) {
            return item.Name + Id + item.RowId;
        }
    }
}