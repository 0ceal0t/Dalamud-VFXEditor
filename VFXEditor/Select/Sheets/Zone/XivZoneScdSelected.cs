using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivZoneScdSelected {
        public readonly XivZone Zone;
        public readonly bool IsSituation;
        public readonly string Path;
        public readonly string SituationDay;
        public readonly string SituationNight;
        public readonly string SituationBattle;
        public readonly string SituationDaybreak;

        public XivZoneScdSelected( XivZone zone ) {
            Zone = zone;
            IsSituation = zone.BgmId >= 1000;
            PluginLog.Log( $"{IsSituation} {zone.BgmId}" );
            if( !IsSituation ) {
                Path = Plugin.DataManager.GetExcelSheet<BGM>().GetRow( zone.BgmId ).File.ToString();
            }
            else {
                var situation = Plugin.DataManager.GetExcelSheet<BGMSituation>().GetRow( zone.BgmId );
                SituationDay = situation.DaytimeID?.Value.File.ToString();
                SituationNight = situation.NightID?.Value.File.ToString();
                SituationBattle = situation.BattleID?.Value.File.ToString();
                SituationDaybreak = situation.DaybreakID?.Value.File.ToString();
            }
        }
    }
}
