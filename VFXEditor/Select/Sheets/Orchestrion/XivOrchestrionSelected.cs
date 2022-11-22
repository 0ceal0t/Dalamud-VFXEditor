using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Rows {
    public class XivOrchestrionSelected {
        public readonly XivOrchestrion Orchestrion;
        public readonly string Path;

        public XivOrchestrionSelected( XivOrchestrion orchestrion ) {
            Orchestrion = orchestrion;
            var pathRow = Plugin.DataManager.GetExcelSheet<OrchestrionPath>().GetRow( ( uint )orchestrion.RowId );
            Path = pathRow.File.ToString();
        }
    }
}
