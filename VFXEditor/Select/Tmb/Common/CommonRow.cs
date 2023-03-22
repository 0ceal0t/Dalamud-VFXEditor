namespace VfxEditor.Select.Tmb.Common {
    public class CommonRow {
        public string Name;
        public string Path;
        public int RowId;

        public CommonRow( int rowId, string path, string name ) {
            RowId = rowId;
            Path = path;
            Name = name;
        }
    }
}
