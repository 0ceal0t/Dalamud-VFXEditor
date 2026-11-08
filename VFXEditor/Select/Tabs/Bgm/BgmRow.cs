using Lumina.Excel.Sheets;
using System.Collections.Generic;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Bgm {
    public class BgmRow : ISelectItem {
        public readonly string Name;
        public readonly int RowId;
        public readonly string Path;

        public BgmRow( BGM bgm, Dictionary<int, string> idToTitle ) {
            Path = bgm.File.ToString();
            RowId = ( int )bgm.RowId;
            Name = ( idToTitle.TryGetValue( RowId, out var title ) && !string.IsNullOrEmpty( title ) ) ? title : Path.Replace( "music/", "" ).Replace( ".scd", "" );
        }

        public string GetName() => Name;
    }
}