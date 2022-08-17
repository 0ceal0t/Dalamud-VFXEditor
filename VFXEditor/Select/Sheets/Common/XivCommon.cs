namespace VFXEditor.Select.Rows {
    public class XivCommon {
        public string Name;
        public ushort Icon;
        public string Path;
        public int RowId;

        public XivCommon( int rowId, string path, string name, ushort icon ) {
            RowId = rowId;
            Path = path;
            Name = name;
            Icon = icon;
        }

        public XivCommon( Lumina.Excel.GeneratedSheets.VFX vfx ) {
            RowId = ( int )vfx.RowId;
            Icon = 0;
            Name = vfx.Location.ToString();
            Path = $"vfx/common/eff/{Name}.avfx";
        }
    }
}
