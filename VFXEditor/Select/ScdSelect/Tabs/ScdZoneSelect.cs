using Dalamud.Interface;
using ImGuiNET;
using VfxEditor.Select.Rows;
using VfxEditor.Utils;

namespace VfxEditor.Select.ScdSelect {
    public class ScdZoneSelect : SelectTab<XivZone, XivZoneScdSelected> {
        public ScdZoneSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.ZoneScd, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            if( Loaded.IsSituation ) {
                DrawPath( "Daytime BGM Path", Loaded.SituationDay, $"{parentId}/Day", SelectResultType.GameZone,  $"{Loaded.Zone.Name} / Day" );
                DrawPath( "Nighttime BGM Path", Loaded.SituationNight, $"{parentId}/Night", SelectResultType.GameZone, $"{Loaded.Zone.Name} / Night" );
                DrawPath( "Battle BGM Path", Loaded.SituationBattle, $"{parentId}/Battle", SelectResultType.GameZone, $"{Loaded.Zone.Name} / Battle" );
                DrawPath( "Daybreak BGM Path", Loaded.SituationDaybreak, $"{parentId}/Break", SelectResultType.GameZone, $"{Loaded.Zone.Name} / Break" );
            }
            else {
                DrawPath( "BGM Path", Loaded.Path, parentId, SelectResultType.GameZone, Loaded.Zone.Name );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 20 );
            UiUtils.IconText( FontAwesomeIcon.InfoCircle, true );
            ImGui.SameLine();
            DisplayPath( "Note: some zones only get their final music after a certain quest. Check the \"Quest BGM\" tab if this is the case." );
        }

        protected override string GetName( XivZone item ) => item.Name;
    }
}