using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Select.Tabs.Gimmick {
    public class GimmickTab : SelectTab<GimmickRow, ParsedPaths> {
        public GimmickTab( SelectDialog dialog, string name ) : base( dialog, name, "Gimmick" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var territories = Dalamud.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
            var suffixToName = new Dictionary<string, string>();
            foreach( var zone in territories ) {
                suffixToName[zone.Name.ToString()] = zone.PlaceName.Value?.Name.ToString();
            }

            var sheet = Dalamud.DataManager.GetExcelSheet<ActionTimeline>().Where( x => x.Key.ToString().Contains( "gimmick" ) );
            foreach( var item in sheet ) {
                Items.Add( new GimmickRow( item, suffixToName ) );
            }
        }

        public override void LoadSelection( GimmickRow item, out ParsedPaths loaded ) => ParsedPaths.ReadFile( item.TmbPath, SelectDataUtils.AvfxRegex, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected() {
            using( var _ = ImRaii.PushId( "CopyTmb" ) ) {
                SelectUiUtils.Copy( Selected.TmbPath );
            }

            ImGui.SameLine();
            ImGui.Text( "TMB:" );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.TmbPath );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Dialog.DrawPaths( Loaded.Paths, Selected.Name, SelectResultType.GameGimmick );
        }
    }
}