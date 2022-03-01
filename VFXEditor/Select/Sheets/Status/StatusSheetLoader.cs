using Lumina.Excel.GeneratedSheets;
using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class StatusSheetLoader : SheetLoader<XivStatus, XivStatus> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                var i = new XivStatus( item );
                if( i.VfxExists ) {
                    Items.Add( i );
                }
            }
        }

        public override bool SelectItem( XivStatus item, out XivStatus selectedItem ) {
            selectedItem = item;
            return true;
        }
    }
}
