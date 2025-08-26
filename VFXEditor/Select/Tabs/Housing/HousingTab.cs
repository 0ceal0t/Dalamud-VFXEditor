using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using System.Linq;

namespace VfxEditor.Select.Tabs.Housing {
    public class HousingTab : SelectTab<HousingRow, ParsedPaths> {
        public HousingTab( SelectDialog dialog, string name ) : base( dialog, name, "Housing" ) { }

        // ===== LOADING =====

        public override void LoadData() {
            var indoorSheet = Dalamud.DataManager.GetExcelSheet<HousingFurniture>().Where( x => x.ModelKey > 0 );
            foreach( var item in indoorSheet ) Items.Add( new HousingRow( item ) );

            var outdoorSheet = Dalamud.DataManager.GetExcelSheet<HousingYardObject>().Where( x => x.ModelKey > 0 );
            foreach( var item in outdoorSheet ) Items.Add( new HousingRow( item ) );
        }

        public override void LoadSelection( HousingRow item, out ParsedPaths loaded ) => ParsedPaths.ReadFile( item.SgbPath, SelectDataUtils.AvfxRegex, out loaded );

        // ===== DRAWING ======

        protected override void DrawSelected() {
            ImGui.Text( "SGB:" );
            ImGui.SameLine();
            SelectUiUtils.DisplayPath( Selected.SgbPath );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            Dialog.DrawPaths( Loaded.Paths, Selected.Name, SelectResultType.GameHousing );
        }
    }
}