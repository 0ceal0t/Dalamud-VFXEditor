using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class ActionTmbSheetLoader : SheetLoader<XivActionTmb, XivActionTmb> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.AffectsPosition );
            foreach( var item in sheet ) {
                Items.Add( new XivActionTmb( item ) );
            }
        }

        public override bool SelectItem( XivActionTmb item, out XivActionTmb selectedItem ) {
            selectedItem = item;
            return true;
        }
    }
}
