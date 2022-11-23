using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxZoneSelect : SelectTab<XivZone, XivZoneSelected> {
        public VfxZoneSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Zones, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            ImGui.Text( "LGB Path: " );
            ImGui.SameLine();
            DisplayPath( Loaded.Zone.LgbPath );

            DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameZone, Loaded.Zone.Name, true );
        }

        protected override string GetName( XivZone item ) => item.Name;
    }
}