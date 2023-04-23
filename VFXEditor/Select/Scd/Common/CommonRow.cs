using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Scd.Common {
    public class CommonRow {
        public readonly string Name;
        public readonly string Path;
        public readonly int RowId;

        public CommonRow( int rowId, string path, string name ) {
            RowId = rowId;
            Path = path;
            Name = name;
        }
    }
}
