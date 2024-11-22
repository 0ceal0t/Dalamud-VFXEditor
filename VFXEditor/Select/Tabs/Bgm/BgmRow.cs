using Lumina.Excel.Sheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Bgm {
    public class BgmRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;
        public readonly string Path;

        public BgmRow( BGM bgm ) {
            Path = bgm.File.ToString();
            Name = Path.Replace( "music/", "" ).Replace( ".scd", "" );
            RowId = ( int )bgm.RowId;
        }

        public string GetName() => Name;
    }
}