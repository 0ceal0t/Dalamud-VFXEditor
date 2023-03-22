namespace VfxEditor.Select2.Scd.Orchestrion {
    public class OrchestrionRow {
        public readonly string Name;
        public readonly int RowId;

        public OrchestrionRow( Lumina.Excel.GeneratedSheets.Orchestrion orchestrion ) {
            Name = orchestrion.Name.ToString();
            RowId = ( int )orchestrion.RowId;
        }
    }
}
