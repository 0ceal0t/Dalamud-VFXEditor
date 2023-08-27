using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System.Linq;
using VfxEditor.Select.Shared;

namespace VfxEditor.Select.Vfx.Housing {
    public class HousingTab : SelectTab<HousingRow, ParseAvfx> {
        public HousingTab( SelectDialog dialog, string name ) : base( dialog, name, "Vfx-Housing", SelectResultType.GameHousing ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var indoorSheet = Plugin.DataManager.GetExcelSheet<HousingFurniture>().Where( x => x.ModelKey > 0 );
            foreach( var item in indoorSheet ) Items.Add( new HousingRow( item ) );

            var outdoorSheet = Plugin.DataManager.GetExcelSheet<HousingYardObject>().Where( x => x.ModelKey > 0 );
            foreach( var item in outdoorSheet ) Items.Add( new HousingRow( item ) );
        }

        public override void LoadSelection( HousingRow item, out ParseAvfx loaded ) => ParseAvfx.ReadFile( item.SgbPath, out loaded );

        // ===== DRAWING ======

        protected override void OnSelect() => LoadIcon( Selected.Icon );

        protected override void DrawSelected() {
            SelectUiUtils.DrawIcon( Icon );
            ImGui.Text( "SGB:" );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.SgbPath );

            DrawPaths( "VFX", Loaded.VfxPaths, Selected.Name, true );
        }

        protected override string GetName( HousingRow item ) => item.Name;
    }
}
