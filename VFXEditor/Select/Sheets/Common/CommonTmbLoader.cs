using System.IO;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class CommonTmbLoader : SheetLoader<XivCommon> {
        public override void OnLoad() {
            var lineIdx = 0;
            foreach( var line in File.ReadLines( SheetManager.MiscTmbPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                Items.Add( new XivCommon( lineIdx, line, line.Replace( "chara/action/", "" ).Replace( ".tmb", "" ), 0 ) );
                lineIdx++;
            }
        }
    }
}