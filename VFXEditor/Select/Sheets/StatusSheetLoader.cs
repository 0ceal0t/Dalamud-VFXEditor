using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class StatusSheetLoader : SheetLoader<XivStatus, XivStatus> {
        public StatusSheetLoader( SheetManager manager, DalamudPluginInterface pi ) : base( manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in _sheet ) {
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
