using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class HousingSheetLoader : SheetLoader<XivHousing, XivHousingSelected> {
        public HousingSheetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<HousingFurniture>().Where( x => x.ModelKey > 0 );
            foreach( var item in _sheet ) {
                Items.Add(new XivHousing( item ));
            }

            var _sheet2 = _plugin.PluginInterface.Data.GetExcelSheet<HousingYardObject>().Where( x => x.ModelKey > 0 );
            foreach( var item in _sheet2 ) {
                Items.Add(new XivHousing( item ));
            }
        }

        public override bool SelectItem( XivHousing item, out XivHousingSelected selectedItem ) {
            selectedItem = null;
            string sgbPath = item.GetSbgPath();
            bool result = _plugin.PluginInterface.Data.FileExists( sgbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( sgbPath );
                    selectedItem = new XivHousingSelected( item, file );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error loading SGB file " + sgbPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
