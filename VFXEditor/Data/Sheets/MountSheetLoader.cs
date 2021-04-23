using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data.Sheets {
    public class MountSheeetLoader : SheetLoader<XivMount, XivMountSelected> {
        public MountSheeetLoader( DataManager manager, Plugin plugin) : base(manager, plugin ) {
        }

        public override void OnLoad() {
            var _sheet = _plugin.PluginInterface.Data.GetExcelSheet<Mount>().Where( x => !string.IsNullOrEmpty(x.Singular) );
            foreach( var item in _sheet ) {
                Items.Add(new XivMount( item ));
            }
        }

        public override bool SelectItem( XivMount item, out XivMountSelected selectedItem ) {
            selectedItem = null;
            string imcPath = item.GetImcPath();
            bool result = _plugin.PluginInterface.Data.FileExists( imcPath );
            if( result ) {
                try {
                    var file = _plugin.PluginInterface.Data.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
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
