using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.Gimmick {
    public class GimmickTab : SelectTab<GimmickRow, ParsedPaths> {
        public GimmickTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Gimmick", SelectResultType.GameGimmick ) { }

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

            DrawPaths( "VFX", Loaded.Paths, Selected.Name, true );
        }

        protected override string GetName( GimmickRow item ) => item.Name;
    }
}
