namespace VfxEditor.Select.Scd.Orchestrion {
    public class OrchestrionRow {
        public readonly string Name;
        public readonly int RowId;

        public OrchestrionRow( Lumina.Excel.GeneratedSheets.Orchestrion orchestrion ) {
            Name = orchestrion.Name.ToString();
            RowId = ( int )orchestrion.RowId;
        }
    }
}
