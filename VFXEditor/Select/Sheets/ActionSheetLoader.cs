using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class ActionSheetLoader : SheetLoader<XivActionBase, XivActionSelected> {
        public ActionSheetLoader(SheetManager manager, DalamudPluginInterface pi) : base(manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.IsPlayerAction );
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
            bool result = _pi.Data.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = _pi.Data.GetFile( tmbPath );
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
