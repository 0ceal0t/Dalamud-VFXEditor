using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivAction : XivActionBase {
        public XivAction( Lumina.Excel.GeneratedSheets.Action action, bool justSelf = false ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            SelfKey = action.AnimationEnd?.Value?.Key.ToString();
            SelfKeyExists = !string.IsNullOrEmpty( SelfKey );

            if( !justSelf ) {
                Castvfx = action.VFX?.Value?.VFX.Value?.Location;
                CastKeyExists = !string.IsNullOrEmpty( Castvfx );

                //startVfx = action.AnimationStart.Value?.VFX.Value?.Location;

                // split this off into its own item
                HitKey = action.ActionTimelineHit?.Value?.Key.ToString();
                HitKeyExists = !string.IsNullOrEmpty( HitKey );
                if( HitKeyExists ) {
                    var sAction = new Lumina.Excel.GeneratedSheets.Action {
                        Icon = action.Icon,
                        Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) ),
                        IsPlayerAction = action.IsPlayerAction,
                        RowId = action.RowId,
                        AnimationEnd = action.ActionTimelineHit
                    };
                    HitAction = new XivAction( sAction, justSelf: true );
                }
            }

            KeyExists = ( CastKeyExists || SelfKeyExists );
        }
    }
}
