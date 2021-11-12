using Dalamud.Logging;
using System;
using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class TmbSheetLoader : SheetLoader<XivTmb, XivTmb> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.AffectsPosition );
            foreach( var item in sheet ) {
                Items.Add( new XivTmb( item ) );
            }
        }

        public override bool SelectItem( XivTmb item, out XivTmb selectedItem ) {
            selectedItem = item;
            return true;
        }
    }
}
