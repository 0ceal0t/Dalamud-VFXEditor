using System.Linq;
using VFXEditor.Select.Rows;

namespace VFXEditor.Select.Sheets {
    public class NonPlayerActionSheetLoader : ActionSheetLoader {
        public override void OnLoad() {
            var sheet = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !( x.IsPlayerAction || x.ClassJob.Value != null ) );
            foreach( var item in sheet ) {
                var action = new XivActionNonPlayer( item );
                if( !action.IsPlaceholder ) {
                    if( action.HitVFXExists ) {
                        Items.Add( ( XivActionNonPlayer )action.HitAction );
                    }
                    if( action.VfxExists ) {
                        Items.Add( action );
                    }
                }
            }
        }
    }
}
