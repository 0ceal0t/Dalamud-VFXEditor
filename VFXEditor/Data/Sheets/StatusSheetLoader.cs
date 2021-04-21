using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class StatusSheetLoader : SheetLoader<XivStatus, XivStatus> {
        public StatusSheetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<Status>().Where( x => !string.IsNullOrEmpty( x.Name ) );
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
