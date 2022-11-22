using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdZoneSelect : ScdSelectTab<XivZone, XivZoneScdSelected> {
        public ScdZoneSelect( string parentId, string tabId, ScdSelectDialog dialog ) : base( parentId, tabId, SheetManager.ZoneScd, dialog ) { }

        protected override bool CheckMatch( XivZone item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void DrawSelected( XivZoneScdSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Zone.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( loadedItem.IsSituation ) {
                DrawPath( "Daytime BGM Path", loadedItem.SituationDay, Id, Dialog, SelectResultType.GameZone, "ZONE", loadedItem.Zone.Name + " / Day" );
                DrawPath( "Nighttime BGM Path", loadedItem.SituationNight, Id, Dialog, SelectResultType.GameZone, "ZONE", loadedItem.Zone.Name + " / Night" );
                DrawPath( "Battle BGM Path", loadedItem.SituationBattle, Id, Dialog, SelectResultType.GameZone, "ZONE", loadedItem.Zone.Name + " / Battle" );
                DrawPath( "Daybreak BGM Path", loadedItem.SituationDaybreak, Id, Dialog, SelectResultType.GameZone, "ZONE", loadedItem.Zone.Name + " / Daybreak" );
            }
            else {
                DrawPath( "BGM Path", loadedItem.Path, Id, Dialog, SelectResultType.GameZone, "ZONE", loadedItem.Zone.Name );
            }
        }

        protected override string UniqueRowTitle( XivZone item ) => $"{item.Name}##{item.RowId}";
    }
}