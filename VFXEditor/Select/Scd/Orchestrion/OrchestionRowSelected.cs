using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Scd.Orchestrion {
    public class OrchestionRowSelected {
        public readonly string Path;

        public OrchestionRowSelected( OrchestrionRow orchestrion ) {
            var pathRow = Dalamud.DataManager.GetExcelSheet<OrchestrionPath>().GetRow( ( uint )orchestrion.RowId );
            Path = pathRow.File.ToString();
        }
    }
}
