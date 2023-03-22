namespace VfxEditor.Select.Scd.Bgm {
    public class BgmRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly string Path;

        public BgmRow( Lumina.Excel.GeneratedSheets.BGM bgm ) {
            Path = bgm.File.ToString();
            Name = Path.Replace( "music/", "" ).Replace( ".scd", "" );
            RowId = ( int )bgm.RowId;
        }
    }
}
