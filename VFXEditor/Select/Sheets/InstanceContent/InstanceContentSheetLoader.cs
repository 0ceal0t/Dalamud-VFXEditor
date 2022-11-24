using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class InstanceContentSheetLoader : SheetLoader<XivInstanceContent, XivInstanceContentSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<ContentFinderCondition>().Where( x => !string.IsNullOrEmpty( x.Name ) && x.Content > 0 && x.ContentLinkType == 1 );
            foreach( var item in sheet ) Items.Add( new XivInstanceContent( item ) );
        }

        public override bool SelectItem( XivInstanceContent item, out XivInstanceContentSelected selectedItem ) {
            selectedItem = new( item );
            return true;
        }
    }
}
