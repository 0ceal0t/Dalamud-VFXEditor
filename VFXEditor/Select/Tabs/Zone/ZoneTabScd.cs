using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Tabs.BgmQuest;

namespace VfxEditor.Select.Tabs.Zone {
    public class SelectedScd {
        public BgmSituationStruct Situation;
        public Dictionary<string, BgmSituationStruct> Quests = [];
    }

    public class ZoneTabScd : ZoneTab<SelectedScd> {
        public ZoneTabScd( SelectDialog dialog, string name ) : base( dialog, name, "Zone-Scd" ) { }

        // ===== LOADING =====

        public override void LoadSelection( ZoneRow item, out SelectedScd loaded ) {
            loaded = new() {
                Situation = BgmQuestTab.GetBgmSituation( item.BgmId )
            };
            if( item.BgmId <= 50000 ) return;

            foreach( var bgmSwitch in Dalamud.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.RowId == item.BgmId ) ) {
                var questName = bgmSwitch.Quest.Value?.Name.ToString();
                var situation = BgmQuestTab.GetBgmSituation( bgmSwitch.BGM );
                loaded.Quests[string.IsNullOrEmpty( questName ) ? item.Name : questName] = situation;
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            DrawBgmSituation( Selected.Name, Loaded.Situation );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            var idx = 0;
            foreach( var entry in Loaded.Quests ) {
                using var _ = ImRaii.PushId( idx );

                if( ImGui.CollapsingHeader( entry.Key ) ) {
                    using var indent = ImRaii.PushIndent();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                    DrawBgmSituation( entry.Key, entry.Value );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                }
                idx++;
            }
        }
    }
}
