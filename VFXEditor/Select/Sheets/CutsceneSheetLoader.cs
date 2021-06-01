using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class CutsceneSheetLoader : SheetLoader<XivCutscene, XivCutsceneSelected> {
        public CutsceneSheetLoader( SheetManager manager, DalamudPluginInterface pluginInterface ) : base( manager, pluginInterface ) {
        }

        public override void OnLoad() {
            var sheet = PluginInterface.Data.GetExcelSheet<Cutscene>().Where( x => !string.IsNullOrEmpty( x.Path ) );
            foreach( var item in sheet ) {
                Items.Add( new XivCutscene( item ) );
            }
        }

        public override bool SelectItem( XivCutscene item, out XivCutsceneSelected selectedItem ) {
            selectedItem = null;
            bool result = PluginInterface.Data.FileExists( item.Path );
            if( result ) {
                try {
                    var file = PluginInterface.Data.GetFile( item.Path );
                    selectedItem = new XivCutsceneSelected( item, file );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error Reading CUTB file " + item.Path, e );
                    return false;
                }
            }
            else {
                PluginLog.Log( item.Path + " does not exist" );
            }
            return result;
        }
    }
}
