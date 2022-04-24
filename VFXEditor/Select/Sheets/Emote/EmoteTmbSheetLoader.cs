using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class EmoteTmbSheetLoader : SheetLoader<XivEmoteTmb, XivEmoteTmb> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
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
