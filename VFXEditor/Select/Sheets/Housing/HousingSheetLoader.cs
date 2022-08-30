using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class HousingSheetLoader : SheetLoader<XivHousing, XivHousingSelected> {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<HousingFurniture>().Where( x => x.ModelKey > 0 );
            foreach( var item in sheet ) {
                Items.Add( new XivHousing( item ) );
            }

            var sheet2 = VfxEditor.DataManager.GetExcelSheet<HousingYardObject>().Where( x => x.ModelKey > 0 );
            foreach( var item in sheet2 ) {
                Items.Add( new XivHousing( item ) );
            }
        }

        public override bool SelectItem( XivHousing item, out XivHousingSelected selectedItem ) {
            selectedItem = null;
            var sgbPath = item.GetSbgPath();
            var result = VfxEditor.DataManager.FileExists( sgbPath );
            if( result ) {
                try {
                    var file = VfxEditor.DataManager.GetFile( sgbPath );
                    selectedItem = new XivHousingSelected( item, file );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error loading SGB file " + sgbPath );
                    return false;
                }
            }
            return result;
        }
    }
}
