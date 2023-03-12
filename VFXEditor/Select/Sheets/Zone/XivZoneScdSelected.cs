using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivZoneScdSelected {
        public readonly XivZone Zone;
        public readonly BgmSituationStruct Situation;
        public readonly Dictionary<string, BgmSituationStruct> Quests = new();

        public XivZoneScdSelected( XivZone zone ) {
            Zone = zone;
            Situation = XivBgmQuestSelected.GetBgmSituation( zone.BgmId );

            if( zone.BgmId <= 50000 ) return;

            foreach( var bgmSwitch in Plugin.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.RowId == zone.BgmId ) ) {
                var questName = bgmSwitch.Quest.Value?.Name.ToString();
                var situation = XivBgmQuestSelected.GetBgmSituation( bgmSwitch.BGM );
                Quests[string.IsNullOrEmpty( questName ) ? Zone.Name : questName] = situation;
            }
        }
    }
}
