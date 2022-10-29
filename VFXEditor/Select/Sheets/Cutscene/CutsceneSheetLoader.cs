using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class CutsceneSheetLoader : SheetLoader<XivCutscene, XivCutsceneSelected> {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );
            foreach( var item in sheet ) {
                Items.Add( new XivCutscene( item ) );
            }
        }

        public override bool SelectItem( XivCutscene item, out XivCutsceneSelected selectedItem ) {
            selectedItem = null;
            var result = VfxEditor.DataManager.FileExists( item.Path );
            if( result ) {
                try {
                    var file = VfxEditor.DataManager.GetFile( item.Path );
                    selectedItem = new XivCutsceneSelected( item, file );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error Reading CUTB file " + item.Path );
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
