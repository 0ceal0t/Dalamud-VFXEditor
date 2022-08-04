using Dalamud.Logging;
using System;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class ActionSheetLoader : SheetLoader<XivActionBase, XivActionSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );
            foreach( var item in sheet ) {
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
            var tmbPath = item.GetTmbPath();
            var result = Plugin.DataManager.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = Plugin.DataManager.GetFile( tmbPath );
                    selectedItem = new XivActionSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error reading TMB " + tmbPath);
                    return false;
                }
            }
            else {
                PluginLog.Error( tmbPath + " does not exist" );
            }
            return result;
        }
    }
}
