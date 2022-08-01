using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class MountSheeetLoader : SheetLoader<XivMount, XivMountSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Mount>().Where( x => !string.IsNullOrEmpty( x.Singular ) );
            foreach( var item in sheet ) {
                Items.Add( new XivMount( item ) );
            }
        }

        public override bool SelectItem( XivMount item, out XivMountSelected selectedItem ) {
            selectedItem = null;
            var imcPath = item.GetImcPath();
            var result = Plugin.DataManager.FileExists( imcPath );
            if( result ) {
                try {
                    var file = Plugin.DataManager.GetFile<Lumina.Data.Files.ImcFile>( imcPath );
                    selectedItem = new XivMountSelected( file, item );
                }
                catch( Exception e ) {
                    PluginLog.Error( "Error loading IMC file " + imcPath, e );
                    return false;
                }
            }
            return result;
        }
    }
}
