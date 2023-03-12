using Dalamud.Logging;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class ActionSheetLoader : SheetLoader<XivAction, XivActionSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) {
                var actionItem = new XivAction( item, false );
                if( actionItem.HasVfx ) Items.Add( actionItem );
                if( actionItem.HitAction != null ) Items.Add( actionItem.HitAction );
            }
        }

        public override bool SelectItem( XivAction item, out XivActionSelected selectedItem ) {
            selectedItem = null;
            if( string.IsNullOrEmpty( item.SelfTmbKey ) ) { // no need to get the file
                selectedItem = new XivActionSelected( null, item );
                return true;
            }

            var tmbPath = item.TmbPath;
            var result = Plugin.DataManager.FileExists( tmbPath );
            if( result ) {
                try {
                    selectedItem = new XivActionSelected( Plugin.DataManager.GetFile( tmbPath ), item );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error reading TMB " + tmbPath );
                    return false;
                }
            }
            else PluginLog.Error( tmbPath + " does not exist" );
            return result;
        }
    }
}
