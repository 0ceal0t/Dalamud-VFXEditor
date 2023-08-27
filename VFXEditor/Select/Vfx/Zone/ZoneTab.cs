using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using VfxEditor.Select.Shared;
using VfxEditor.Select.Shared.Zone;

namespace VfxEditor.Select.Vfx.Zone {
    public class ZoneTab : SelectTab<ZoneRow, ParseAvfx> {
        public ZoneTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Zone", SelectResultType.GameZone ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var sheet = Plugin.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) Items.Add( new ZoneRow( item ) );
        }

        public override void LoadSelection( ZoneRow item, out ParseAvfx loaded ) => ParseAvfx.ReadFile( item.LgbPath, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected() {
            ImGui.Text( "LGB: " );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.LgbPath );

            DrawPaths( "VFX", Loaded.VfxPaths, Selected.Name, true );
        }

        protected override string GetName( ZoneRow item ) => item.Name;
    }
}
