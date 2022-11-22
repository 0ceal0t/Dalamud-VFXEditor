namespace VfxEditor.Select.Rows {
    public class XivOrchestrion {
        public readonly string Name;
        public readonly int RowId;

        public XivOrchestrion( Lumina.Excel.GeneratedSheets.Orchestrion orchestrion ) {
            Name = orchestrion.Name.ToString();
            RowId = ( int )orchestrion.RowId;
        }
    }
}
