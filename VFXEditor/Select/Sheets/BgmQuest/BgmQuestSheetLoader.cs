using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class BgmQuestSheetLoader : SheetLoader<XivBgmQuest, XivBgmQuestSelected> {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<BGMSwitch>().Where( x => x.Quest.Row > 0 );
            foreach( var item in sheet ) Items.Add( new XivBgmQuest( item ) );
        }

        public override bool SelectItem( XivBgmQuest item, out XivBgmQuestSelected selectedItem ) {
            selectedItem = new( item );
            return true;
        }
    }
}
