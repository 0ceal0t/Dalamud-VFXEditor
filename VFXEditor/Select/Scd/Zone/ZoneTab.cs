using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Scd.BgmQuest;
using VfxEditor.Select.Shared.Zone;

namespace VfxEditor.Select.Scd.Zone {
    public class ZoneRowSelected {
        public BgmSituationStruct Situation;
        public Dictionary<string, BgmSituationStruct> Quests = new();
    }

    public class ZoneTab : SelectTab<ZoneRow, ZoneRowSelected> {
        public ZoneTab( SelectDialog dialog, string name ) : base( dialog, name, "Scd-Zone", SelectResultType.GameZone ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.BGM != 0 && x.BGM != 1001 && x.BGM != 1000 );
            foreach( var item in sheet ) Items.Add( new ZoneRow( item ) );
        }

        public override void LoadSelection( ZoneRow item, out ZoneRowSelected loaded ) {
            loaded = new() {
                Situation = BgmQuestTab.GetBgmSituation( item.BgmId )
            };
            if( item.BgmId <= 50000 ) return;

            foreach( var bgmSwitch in Plugin.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.RowId == item.BgmId ) ) {
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

        protected override string GetName( ZoneRow item ) => item.Name;
    }
}
