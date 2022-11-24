using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class ZoneScdSheetLoader : SheetLoader<XivZone, XivZoneScdSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<TerritoryType>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.BGM > 0 );
            foreach( var item in sheet ) Items.Add( new XivZone( item ) );
        }

        public override bool SelectItem( XivZone item, out XivZoneScdSelected selectedItem ) {
            selectedItem = new XivZoneScdSelected( item );
            return true;
        }
    }
}
