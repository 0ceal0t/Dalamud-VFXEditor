using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class ActionPapSheetLoader : SheetLoader<XivActionPap, XivActionPapSelected> {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );
            foreach( var item in sheet ) {
                Items.Add( new XivActionPap( item ) );
            }
        }

        public override bool SelectItem( XivActionPap item, out XivActionPapSelected selectedItem ) {
            selectedItem = new XivActionPapSelected(
                item,
                SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( item.StartPap ) ),
                SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( item.EndPap ) ),
                SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( item.HitPap ) )
            );
            return true;
        }
    }
}
