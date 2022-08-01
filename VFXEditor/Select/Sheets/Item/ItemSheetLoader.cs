using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class ItemSheetLoader : SheetLoader<XivItem, XivItemSelected> {
        public override void OnLoad() {
            var _sheet = Plugin.DataManager.GetExcelSheet<Item>().Where( x => x.EquipSlotCategory.Value?.MainHand == 1 || x.EquipSlotCategory.Value?.OffHand == 1 );
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
            var imcPath = item.ImcPath;
            var result = Plugin.DataManager.FileExists( imcPath );
            if( result ) {
                try {
                    var file = Plugin.DataManager.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivItemSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error loading IMC file " + imcPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
