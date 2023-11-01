using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tabs.Orchestrions {
    public class OrchestrionRow {
        public readonly string Name;
        public readonly int RowId;

        public OrchestrionRow( Orchestrion orchestrion ) {
            Name = orchestrion.Name.ToString();
            RowId = ( int )orchestrion.RowId;
        }
    }
}