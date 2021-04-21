using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class ActionSheetLoader : SheetLoader<XivActionBase, XivActionSelected> {
        public ActionSheetLoader(DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.IsPlayerAction );
            foreach( var item in _sheet ) {
                var i = new XivAction( item );
                if( i.VfxExists ) {
                    Items.Add( i );
                }
                if( i.HitVFXExists ) {
                    Items.Add( i.HitAction );
                }
            }
        }

        public override bool SelectItem( XivActionBase item, out XivActionSelected selectedItem ) {
            selectedItem = null;
            if( !item.SelfVFXExists ) { // no need to get the file
                selectedItem = new XivActionSelected( null, item );
                return true;
            }
            string tmbPath = item.GetTmbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( tmbPath );
                    selectedItem = new XivActionSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error reading TMB " + tmbPath, e );
                    return false;
                }
            }
            else {
                PluginLog.Log( tmbPath + " does not exist" );
            }
            return result;
        }
    }
}
