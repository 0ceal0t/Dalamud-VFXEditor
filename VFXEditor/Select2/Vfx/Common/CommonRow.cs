using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select2.Vfx.Common {
    public class CommonRow {
        public string Name;
        public ushort Icon;
        public string Path;
        public int RowId;

        public CommonRow( int rowId, string path, string name, ushort icon ) {
            RowId = rowId;
            Path = path;
            Name = name;
            Icon = icon;
        }

        public CommonRow( VFX vfx ) {
            RowId = ( int )vfx.RowId;
            Icon = 0;
            Name = vfx.Location.ToString();
            Path = $"vfx/common/eff/{Name}.avfx";
        }
    }
}
