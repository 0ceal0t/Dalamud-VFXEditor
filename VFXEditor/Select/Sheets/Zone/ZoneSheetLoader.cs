using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class ZoneSheetLoader : SheetLoader<XivZone, XivZoneSelected> {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                Items.Add( new XivZone( item ) );
            }
        }

        public override bool SelectItem( XivZone item, out XivZoneSelected selectedItem ) {
            selectedItem = null;
            var lgbPath = item.GetLgbPath();
            var result = VfxEditor.DataManager.FileExists( lgbPath );
            if( result ) {
                try {
                    var file = VfxEditor.DataManager.GetFile<Lumina.Data.Files.LgbFile>( lgbPath );
                    selectedItem = new XivZoneSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error reading LGB file " + lgbPath );
                    return false;
                }
            }
            return result;
        }
    }
}
