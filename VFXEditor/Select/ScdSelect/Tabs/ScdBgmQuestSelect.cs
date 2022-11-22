using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdBgmQuestSelect : ScdSelectTab<XivBgmQuest, XivBgmQuestSelected> {
        public ScdBgmQuestSelect( string parentId, string tabId, ScdSelectDialog dialog ) : base( parentId, tabId, SheetManager.BgmQuest, dialog ) { }

        protected override bool CheckMatch( XivBgmQuest item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void DrawSelected( XivBgmQuestSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.BgmQuest.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( loadedItem.IsSituation ) {
                DrawPath( "Daytime BGM Path", loadedItem.SituationDay, Id, Dialog, SelectResultType.GameQuest, "QUEST", loadedItem.BgmQuest.Name + " / Day" );
                DrawPath( "Nighttime BGM Path", loadedItem.SituationNight, Id, Dialog, SelectResultType.GameQuest, "QUEST", loadedItem.BgmQuest.Name + " / Night" );
                DrawPath( "Battle BGM Path", loadedItem.SituationBattle, Id, Dialog, SelectResultType.GameQuest, "QUEST", loadedItem.BgmQuest.Name + " / Battle" );
                DrawPath( "Daybreak BGM Path", loadedItem.SituationDaybreak, Id, Dialog, SelectResultType.GameQuest, "QUEST", loadedItem.BgmQuest.Name + " / Daybreak" );
            }
            else {
                DrawPath( "BGM Path", loadedItem.Path, Id, Dialog, SelectResultType.GameZone, "QUEST", loadedItem.BgmQuest.Name );
            }
        }

        protected override string UniqueRowTitle( XivBgmQuest item ) => $"{item.Name}##{item.RowId}";
    }
}