using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace VfxEditor.Select.Scd.BgmQuest {
    public struct BgmSituationStruct {
        public bool IsSituation;
        public string Path;
        public string DayPath;
        public string NightPath;
        public string BattlePath;
        public string DaybreakPath;
    }

    public class BgmQuestRowSelected {
        public BgmSituationStruct Situation;
    }

    public class BgmQuestTab : SelectTab<BgmQuestRow, BgmQuestRowSelected> {
        public BgmQuestTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-BgmQuest", SelectResultType.GameMusic ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Dalamud.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.Quest.Row > 0 );
            foreach( var item in sheet ) Items.Add( new BgmQuestRow( item ) );
        }

        public override void LoadSelection( BgmQuestRow item, out BgmQuestRowSelected loaded ) {
            loaded = new() {
                Situation = GetBgmSituation( item.BgmId )
            };
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawBgmSituation( Selected.Name, Loaded.Situation );
        }

        protected override string GetName( BgmQuestRow item ) => item.Name;

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
