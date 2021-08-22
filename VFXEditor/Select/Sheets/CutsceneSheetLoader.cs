using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class CutsceneSheetLoader : SheetLoader<XivCutscene, XivCutsceneSelected> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );
            foreach( var item in sheet ) {
                Items.Add( new XivCutscene( item ) );
            }
        }

        public override bool SelectItem( XivCutscene item, out XivCutsceneSelected selectedItem ) {
            selectedItem = null;
            var result = SheetManager.DataManager.FileExists( item.Path );
            if( result ) {
                try {
                    var file = SheetManager.DataManager.GetFile( item.Path );
                    selectedItem = new XivCutsceneSelected( item, file );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error Reading CUTB file " + item.Path, e );
                    return false;
                }
            }
            else {
                PluginLog.Error( item.Path + " does not exist" );
            }
            return result;
        }
    }
}
