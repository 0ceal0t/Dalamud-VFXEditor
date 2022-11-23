using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdBgmQuestSelect : SelectTab<XivBgmQuest, XivBgmQuestSelected> {
        public ScdBgmQuestSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.BgmQuest, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            if( Loaded.IsSituation ) {
                DrawPath( "Daytime BGM Path", Loaded.SituationDay, $"{parentId}/Day", SelectResultType.GameQuest, $"{Loaded.BgmQuest.Name} / Day" );
                DrawPath( "Nighttime BGM Path", Loaded.SituationNight, $"{parentId}/Night", SelectResultType.GameQuest, $"{Loaded.BgmQuest.Name} / Night" );
                DrawPath( "Battle BGM Path", Loaded.SituationBattle, $"{parentId}/Battle", SelectResultType.GameQuest, $"{Loaded.BgmQuest.Name} / Battle" );
                DrawPath( "Daybreak BGM Path", Loaded.SituationDaybreak, $"{parentId}/Break", SelectResultType.GameQuest, $"{Loaded.BgmQuest.Name} / Break" );
            }
            else {
                DrawPath( "BGM Path", Loaded.Path, parentId, SelectResultType.GameZone, Loaded.BgmQuest.Name );
            }
        }

        protected override string GetName( XivBgmQuest item ) => item.Name;
    }
}