using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class EmoteSheetLoader : SheetLoader<XivEmote, XivEmoteSelected> {
        public EmoteSheetLoader( SheetManager manager, DalamudPluginInterface pi ) : base( manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
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
                    var result = _pi.Data.FileExists( path );
                    if( result ) {
                        files.Add( _pi.Data.GetFile( path ) );
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
