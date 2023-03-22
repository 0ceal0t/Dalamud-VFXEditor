using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select2.Vfx.Common {
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

        public CommonRow( VFX vfx ) {
            RowId = ( int )vfx.RowId;
            Icon = 0;
            Name = vfx.Location.ToString();
            Path = $"vfx/common/eff/{Name}.avfx";
        }
    }
}
