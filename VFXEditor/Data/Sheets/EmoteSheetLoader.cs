using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class EmoteSheetLoader : SheetLoader<XivEmote, XivEmoteSelected> {
        public EmoteSheetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in _sheet ) {
                var i = new XivEmote( item );
                if( i.PapFiles.Count > 0 ) {
                    Items.Add( i );
                }
            }
        }

        public override bool SelectItem( XivEmote item, out XivEmoteSelected selectedItem ) {
            selectedItem = null;
            List<Lumina.Data.FileResource> files = new List<Lumina.Data.FileResource>();
            try {
                foreach( string path in item.PapFiles ) {
                    var result = _plugin.PluginInterface.Data.FileExists( path );
                    if( result ) {
                        files.Add( _plugin.PluginInterface.Data.GetFile( path ) );
                    }
                }
                selectedItem = new XivEmoteSelected( item, files );
            }
            catch( Exception e ) {
                PluginLog.LogError( "Error reading emote file", e );
                return false;
            }
            return true;
        }
    }
}
