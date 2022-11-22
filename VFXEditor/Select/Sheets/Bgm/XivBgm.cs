namespace VfxEditor.Select.Rows {
    public class XivBgm {
        public readonly string Name;
        public readonly int RowId;
        public readonly string Path;

        public XivBgm( Lumina.Excel.GeneratedSheets.BGM bgm ) {
            Path = bgm.File.ToString();
            Name = Path.Replace( "music/", "" ).Replace( ".scd", "" );
            RowId = ( int )bgm.RowId;
        }
    }
}
