namespace VfxEditor.Select.Shared.Common {
    public class CommonRow {
        public readonly string Name;
        public readonly ushort Icon;
        public readonly string Path;
        public readonly int RowId;

        public CommonRow( int rowId, string path, string name, ushort icon ) {
            RowId = rowId;
            Path = path;
            Name = name;
            Icon = icon;
        }
    }
}
