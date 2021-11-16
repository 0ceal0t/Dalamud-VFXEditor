using Dalamud.Logging;
using System;
using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class PapSheetLoader : SheetLoader<XivPap, XivPap> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.AffectsPosition );
            foreach( var item in sheet ) {
                Items.Add( new XivPap( item ) );
            }
        }

        public override bool SelectItem( XivPap item, out XivPap selectedItem ) {
            selectedItem = new XivPap(
                item.RowId,
                item.Icon,
                item.Name,
                SheetManager.DataManager.FileExists( item.StartPap ) ? item.StartPap : "",
                SheetManager.DataManager.FileExists( item.EndPap ) ? item.EndPap : "",
                SheetManager.DataManager.FileExists( item.HitPap ) ? item.HitPap : ""
            );
            return true;
        }
    }
}
