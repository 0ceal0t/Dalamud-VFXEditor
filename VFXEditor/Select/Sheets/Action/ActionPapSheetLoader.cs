using Lumina.Excel.GeneratedSheets;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class ActionPapSheetLoader : SheetLoader<XivActionPap, XivActionPapSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && ( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );

            foreach( var item in sheet ) Items.Add( new XivActionPap( item ) );
        }

        public override bool SelectItem( XivActionPap item, out XivActionPapSelected selectedItem ) {
            selectedItem = new XivActionPapSelected(
                item,
                SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( item.Start ) ),
                SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( item.End ) ),
                SheetManager.FileExistsFilter( SheetManager.GetAllSkeletonPaths( item.Hit ) )
            );
            return true;
        }
    }
}
