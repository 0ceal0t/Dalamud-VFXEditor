using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class ItemSheetLoader : SheetLoader<XivItem, XivItemSelected> {
        public ItemSheetLoader( SheetManager manager, DalamudPluginInterface pi ) : base( manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<Item>().Where( x => x.EquipSlotCategory.Value?.MainHand == 1 || x.EquipSlotCategory.Value?.OffHand == 1 );
            foreach( var item in _sheet ) {
                var i = new XivItem( item );
                if( i.HasModel ) {
                    Items.Add( i );
                }
                if( i.HasSub ) {
                    Items.Add( i.SubItem );
                }
            }
        }

        public override bool SelectItem( XivItem item, out XivItemSelected selectedItem ) {
            selectedItem = null;
            string imcPath = item.GetImcPath();
            bool result = _pi.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = _pi.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivItemSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error loading IMC file " + imcPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
