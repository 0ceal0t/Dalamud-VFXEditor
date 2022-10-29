using System.Linq;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.Sheets {
    public class NonPlayerActionSheetLoader : ActionSheetLoader {
        public override void OnLoad() {
            var sheet = VfxEditor.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>()
                .Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );
            foreach( var item in sheet ) {
                var action = new XivActionNonPlayer( item );
                if( !action.IsPlaceholder ) {
                    if( action.HitKeyExists ) {
                        Items.Add( ( XivActionNonPlayer )action.HitAction );
                    }
                    if( action.KeyExists ) {
                        Items.Add( action );
                    }
                }
            }
        }
    }
}
