using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class NonPlayerActionSheetLoader : ActionSheetLoader {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );
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
