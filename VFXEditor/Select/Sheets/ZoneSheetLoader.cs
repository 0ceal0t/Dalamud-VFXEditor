using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class ZoneSheetLoader : SheetLoader<XivZone, XivZoneSelected> {
        public ZoneSheetLoader( SheetManager manager, DalamudPluginInterface pi ) : base( manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in _sheet ) {
                Items.Add( new XivZone( item ) );
            }
        }

        public override bool SelectItem( XivZone item, out XivZoneSelected selectedItem ) {
            selectedItem = null;
            string lgbPath = item.GetLgbPath();
            bool result = _pi.Data.FileExists( lgbPath );
            if( result ) {
                try {
                    var file = _pi.Data.GetFile<Lumina.Data.Files.LgbFile>( lgbPath );
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
