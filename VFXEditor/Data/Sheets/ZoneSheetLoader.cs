using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class ZoneSheetLoader : SheetLoader<XivZone, XivZoneSelected> {
        public ZoneSheetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in _sheet ) {
                Items.Add( new XivZone( item ) );
            }
        }

        public override bool SelectItem( XivZone item, out XivZoneSelected selectedItem ) {
            selectedItem = null;
            string lgbPath = item.GetLgbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( lgbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.LgbFile>( lgbPath );
                    selectedItem = new XivZoneSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error reading LGB file " + lgbPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
