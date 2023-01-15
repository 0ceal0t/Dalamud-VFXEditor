using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class NonPlayerActionSheetLoader : ActionSheetLoader {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );

            foreach( var item in sheet ) {
                var action = new XivAction( item, false );
                if( action.HasVfx ) Items.Add( action );
                if( action.HitAction != null ) Items.Add( action.HitAction );
            }
        }
    }
}
