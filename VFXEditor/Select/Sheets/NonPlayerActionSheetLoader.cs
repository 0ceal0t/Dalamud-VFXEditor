using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Data.Rows;

namespace VFXSelect.Data.Sheets {
    public class NonPlayerActionSheetLoader : ActionSheetLoader {
        public NonPlayerActionSheetLoader( SheetManager manager, DalamudPluginInterface pi ) : base( manager, pi ) {
        }

        public override void OnLoad() {
            var _sheet = _pi.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where( x => !string.IsNullOrEmpty( x.Name ) && !x.IsPlayerAction );
            foreach( var item in _sheet ) {
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
