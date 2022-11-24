using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivAction : XivActionBase {
        public XivAction( Action action, bool justSelf = false ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            SelfTmbKey = action.AnimationEnd?.Value?.Key.ToString();
            if( justSelf ) return;

            CastVfxKey = action.VFX?.Value?.VFX.Value?.Location;

            // split this off into its own item
            var hitKey = action.ActionTimelineHit?.Value?.Key.ToString();
            if( string.IsNullOrEmpty( hitKey ) ) return;
            var hitAction = new Action {
                Icon = action.Icon,
                Name = new SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) ),
                IsPlayerAction = action.IsPlayerAction,
                RowId = action.RowId,
                AnimationEnd = action.ActionTimelineHit
            };
            HitAction = new XivAction( hitAction, justSelf: true );
        }
    }
}
