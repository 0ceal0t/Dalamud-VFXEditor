using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
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
            if( item.BgmId <= 50000 ) {
                loaded = new() {
                    Situation = BgmQuestTab.GetBgmSituation( item.BgmId )
                };
            }
            else {
                loaded = new(); // TODO: BgmId > 50000 maps to quest-linked BGM switches, not yet resolved
            }
        }

        // ===== DRAWING ======

        protected override void DrawSelected() {
            Dialog.DrawBgmSituation( Selected.Name, Loaded.Situation, SelectResultType.GameZone );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            var idx = 0;
            foreach( var entry in Loaded.Quests ) {
                using var _ = ImRaii.PushId( idx );

                if( ImGui.CollapsingHeader( entry.Key ) ) {
                    using var indent = ImRaii.PushIndent();
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                    Dialog.DrawBgmSituation( entry.Key, entry.Value, SelectResultType.GameZone );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
                }
                idx++;
            }
        }
    }
}
