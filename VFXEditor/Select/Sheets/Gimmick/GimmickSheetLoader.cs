using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using System.Collections.Generic;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class GimmickSheetLoader : SheetLoader<XivGimmick, XivGimmickSelected> {
        public override void OnLoad() {
            var territories = Plugin.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
            var suffixToName = new Dictionary<string, string>();
            foreach( var zone in territories ) {
                suffixToName[zone.Name.ToString()] = zone.PlaceName.Value?.Name.ToString();
            }

            var sheet = Plugin.DataManager.GetExcelSheet<ActionTimeline>().Where( x => x.Key.ToString().Contains( "gimmick" ) );
            foreach( var item in sheet ) {
                var i = new XivGimmick( item, suffixToName );
                Items.Add( i );
            }
        }

        public override bool SelectItem( XivGimmick item, out XivGimmickSelected selectedItem ) {
            selectedItem = null;
            var tmbPath = item.TmbPath;
            var result = Plugin.DataManager.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = Plugin.DataManager.GetFile( tmbPath );
                    selectedItem = new XivGimmickSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error reading TMB file " + tmbPath);
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
