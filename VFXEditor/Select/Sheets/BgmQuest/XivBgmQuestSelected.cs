using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Rows {
    public class XivBgmQuestSelected {
        public readonly XivBgmQuest BgmQuest;
        public readonly bool IsSituation;
        public readonly string Path;
        public readonly string SituationDay;
        public readonly string SituationNight;
        public readonly string SituationBattle;
        public readonly string SituationDaybreak;

        public XivBgmQuestSelected( XivBgmQuest zone ) {
            BgmQuest = zone;
            IsSituation = zone.BgmId >= 1000;
            if( !IsSituation ) {
                Path = Plugin.DataManager.GetExcelSheet<BGM>().GetRow( zone.BgmId )?.File.ToString();
            }
            else {
                var situation = Plugin.DataManager.GetExcelSheet<BGMSituation>().GetRow( zone.BgmId );
                SituationDay = situation?.DaytimeID?.Value.File.ToString();
                SituationNight = situation?.NightID?.Value.File.ToString();
                SituationBattle = situation?.BattleID?.Value.File.ToString();
                SituationDaybreak = situation?.DaybreakID?.Value.File.ToString();
            }
        }
    }
}
