using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class MountSheeetLoader : SheetLoader<XivMount, XivMountSelected> {
        public MountSheeetLoader( SheetManager manager, DalamudPluginInterface pi ) : base( manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<Mount>().Where( x => !string.IsNullOrEmpty(x.Singular) );
            foreach( var item in _sheet ) {
                Items.Add(new XivMount( item ));
            }
        }

        public override bool SelectItem( XivMount item, out XivMountSelected selectedItem ) {
            selectedItem = null;
            string imcPath = item.GetImcPath();
            bool result = _pi.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = _pi.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivMountSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Error loading IMC file " + imcPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
