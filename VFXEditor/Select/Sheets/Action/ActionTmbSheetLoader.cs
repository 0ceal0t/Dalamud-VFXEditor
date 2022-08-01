using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class ActionTmbSheetLoader : SheetLoader<XivActionTmb, XivActionTmb> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.AffectsPosition );
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
