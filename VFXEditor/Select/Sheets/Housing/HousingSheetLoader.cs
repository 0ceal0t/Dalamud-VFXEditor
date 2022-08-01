using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class HousingSheetLoader : SheetLoader<XivHousing, XivHousingSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<HousingFurniture>().Where( x => x.ModelKey > 0 );
            foreach( var item in sheet ) {
                Items.Add( new XivHousing( item ) );
            }

            var sheet2 = Plugin.DataManager.GetExcelSheet<HousingYardObject>().Where( x => x.ModelKey > 0 );
            foreach( var item in sheet2 ) {
                Items.Add( new XivHousing( item ) );
            }
        }

        public override bool SelectItem( XivHousing item, out XivHousingSelected selectedItem ) {
            selectedItem = null;
            var sgbPath = item.GetSbgPath();
            var result = Plugin.DataManager.FileExists( sgbPath );
            if( result ) {
                try {
                    var file = Plugin.DataManager.GetFile( sgbPath );
                    selectedItem = new XivHousingSelected( item, file );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error loading SGB file " + sgbPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
