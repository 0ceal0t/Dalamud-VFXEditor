using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class GimmickSheetLoader : SheetLoader<XivGimmick, XivGimmickSelected> {
        public GimmickSheetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<ActionTimeline>().Where( x => x.Key.ToString().Contains( "gimmick" ) );

            var territories = _plugin.PluginInterface.Data.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) ).ToList();
            Dictionary<string, string> suffixToName = new Dictionary<string, string>();
            foreach( var _zone in territories ) {
                suffixToName[_zone.Name.ToString()] = _zone.PlaceName.Value?.Name.ToString();
            }

            foreach( var item in _sheet ) {
                var i = new XivGimmick( item, suffixToName );
                Items.Add( i );
            }
        }

        public override bool SelectItem( XivGimmick item, out XivGimmickSelected selectedItem ) {
            selectedItem = null;
            string tmbPath = item.GetTmbPath();
            bool result = _plugin.PluginInterface.Data.FileExists( tmbPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile( tmbPath );
                    selectedItem = new XivGimmickSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error reading TMB file " + tmbPath, e );
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
