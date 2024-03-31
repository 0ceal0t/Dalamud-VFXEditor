using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.BgmQuest {
    public struct BgmSituationStruct {
        public bool IsSituation;
        public string Path;
        public string DayPath;
        public string NightPath;
        public string BattlePath;
        public string DaybreakPath;
    }

    public class SelectedBgmQuest {
        public BgmSituationStruct Situation;
    }

    public class BgmQuestTab : SelectTab<BgmQuestRow, SelectedBgmQuest> {
        public BgmQuestTab( SelectDialog dialog, string name ) : base( dialog, name, "BgmQuest" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.Quest.Row > 0 );
            foreach( var item in sheet ) Items.Add( new BgmQuestRow( item ) );
        }

        public override void LoadSelection( BgmQuestRow item, out SelectedBgmQuest loaded ) {
            loaded = new() {
                Situation = GetBgmSituation( item.BgmId )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawBgmSituation( Selected.Name, Loaded.Situation, SelectResultType.GameMusic );
        }

        public static BgmSituationStruct GetBgmSituation( ushort bgmId ) {
            if( bgmId < 1000 ) {
                return new BgmSituationStruct {
                    Path = Dalamud.DataManager.GetExcelSheet<BGM>().GetRow( bgmId )?.File.ToString(),
                    IsSituation = false
                };
            }
            else {
                var situation = Dalamud.DataManager.GetExcelSheet<BGMSituation>().GetRow( bgmId );
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