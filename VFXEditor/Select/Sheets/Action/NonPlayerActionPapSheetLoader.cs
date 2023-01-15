using System.Collections.Generic;
using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class NonPlayerActionPapSheetLoader : ActionPapSheetLoader {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) && !x.AffectsPosition );

            foreach( var item in sheet ) Items.Add( new XivActionPap( item ) );
        }
    }
}
