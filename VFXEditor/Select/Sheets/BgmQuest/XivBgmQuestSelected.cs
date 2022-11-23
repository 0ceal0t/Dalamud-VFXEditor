using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using NAudio.SoundFont;

namespace VfxEditor.Select.Rows {
    public struct BgmSituationStruct {
        public bool IsSituation;
        public string Path;
        public string DayPath;
        public string NightPath;
        public string BattlePath;
        public string DaybreakPath;
    }

    public class XivBgmQuestSelected {
        public readonly XivBgmQuest BgmQuest;
        public readonly BgmSituationStruct Situation;

        public XivBgmQuestSelected( XivBgmQuest quest ) {
            BgmQuest = quest;
            Situation = GetBgmSituation( quest.BgmId );
        }

        public static BgmSituationStruct GetBgmSituation( ushort bgmId ) {
            if( bgmId < 1000 ) {
                return new BgmSituationStruct {
                    Path = Plugin.DataManager.GetExcelSheet<BGM>().GetRow( bgmId )?.File.ToString(),
                    IsSituation = false
                };
            }
            else {
                var situation = Plugin.DataManager.GetExcelSheet<BGMSituation>().GetRow( bgmId );
                return new BgmSituationStruct {
                    DayPath = situation?.DaytimeID.Value?.File.ToString(),
                    NightPath = situation?.NightID.Value?.File.ToString(),
                    BattlePath = situation?.BattleID.Value?.File.ToString(),
                    DaybreakPath = situation?.DaybreakID.Value?.File.ToString(),
                    IsSituation = true
                };
            }
        }
    }
}
