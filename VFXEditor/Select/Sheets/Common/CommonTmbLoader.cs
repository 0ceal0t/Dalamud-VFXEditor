using System.IO;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class CommonTmbLoader : SheetLoader<XivCommon, XivCommon> {
        public override void OnLoad() {
            var lineIdx = 0;
            foreach( var line in File.ReadLines( SheetManager.MiscTmbPath ).Where( x => !string.IsNullOrEmpty( x ) ) ) {
                Items.Add( new XivCommon( lineIdx, line, line.Replace( "chara/action/", "" ).Replace( ".tmb", "" ), 0 ) );
                lineIdx++;
            }
        }

        public override bool SelectItem( XivCommon item, out XivCommon selectedItem ) {
            selectedItem = item;
            return true;
        }
    }
}