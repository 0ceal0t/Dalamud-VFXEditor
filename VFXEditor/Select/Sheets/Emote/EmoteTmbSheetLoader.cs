using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class EmoteTmbSheetLoader : SheetLoader<XivEmoteTmb, XivEmoteTmb> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                Items.Add( new XivEmoteTmb( item ) );
            }
        }

        public override bool SelectItem( XivEmoteTmb item, out XivEmoteTmb selectedItem ) {
            selectedItem = item;
            return true;
        }
    }
}
