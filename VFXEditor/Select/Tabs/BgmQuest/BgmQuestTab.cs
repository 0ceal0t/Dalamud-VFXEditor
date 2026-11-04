using Lumina.Excel.Sheets;
using System.Collections.Generic;

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
            foreach( var row in Dalamud.DataManager.GetSubrowExcelSheet<BGMSwitch>() ) {
                foreach( var item in row ) {
                    if( item.Quest.RowId == 0 ) continue;
                    var quest = new BgmQuestRow( item );
                    if( !string.IsNullOrEmpty( quest.Name ) ) Items.Add( quest );
                }
            }
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

        public static BgmSituationStruct GetBgmSituation( uint bgmId ) {
            if( Dalamud.DataManager.GetExcelSheet<BGM>().TryGetRow( bgmId, out var bgm ) ) {
                return new BgmSituationStruct {
                    Path = bgm.File.ToString(),
                    IsSituation = false
                };
            }

            if( Dalamud.DataManager.GetExcelSheet<BGMSituation>().TryGetRow( bgmId, out var situation ) ) {
                return new BgmSituationStruct {
                    DayPath = situation.DaytimeID.ValueNullable?.File.ToString(),
                    NightPath = situation.NightID.ValueNullable?.File.ToString(),
                    BattlePath = situation.BattleID.ValueNullable?.File.ToString(),
                    DaybreakPath = situation.DaybreakID.ValueNullable?.File.ToString(),
                    IsSituation = true
                };
            }

            return new BgmSituationStruct();
        }

        public static bool IsBgmSwitch( uint bgmId ) => Dalamud.DataManager.GetSubrowExcelSheet<BGMSwitch>().HasRow( bgmId );

        public static Dictionary<string, BgmSituationStruct> GetBgmSwitch( uint bgmId ) {
            var ret = new Dictionary<string, BgmSituationStruct>();
            if( !Dalamud.DataManager.GetSubrowExcelSheet<BGMSwitch>().TryGetRow( bgmId, out var subrows ) ) return ret;

            foreach( var item in subrows ) {
                var name = item.Quest.RowId == 0 ? "Default" : ( item.Quest.ValueNullable?.Name.ToString() ?? "" );
                if( string.IsNullOrEmpty( name ) ) name = $"Quest {item.Quest.RowId}";

                var key = name;
                for( var i = 2; ret.ContainsKey( key ); i++ ) key = $"{name} ({i})";
                ret[key] = GetBgmSituation( item.BGM.RowId );
            }
            return ret;
        }
    }
}