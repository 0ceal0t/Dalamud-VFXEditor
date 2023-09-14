namespace VfxEditor.Select.Tex.Status {
    public class StatusRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly uint Icon;

        public StatusRow( Lumina.Excel.GeneratedSheets.Status status ) {
            Name = status.Name.ToString();
            RowId = ( int )status.RowId;
            Icon = status.Icon;
        }
    }
}
