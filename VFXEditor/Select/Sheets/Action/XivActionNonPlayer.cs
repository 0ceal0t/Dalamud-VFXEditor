using System.Collections.Generic;
using System.Text;

namespace VfxEditor.Select.Rows {
    public class XivActionNonPlayer : XivActionBase {
        public bool IsPlaceholder = false;
        public List<XivActionNonPlayer> PlaceholderActions;

        public XivActionNonPlayer( Lumina.Excel.GeneratedSheets.Action action, bool justSelf = false, string forceSelfKey = "" ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            if( string.IsNullOrEmpty( forceSelfKey ) ) {
                SelfTmbKey = action.AnimationEnd?.Value?.Key.ToString();
                if( string.IsNullOrEmpty( SelfTmbKey ) || SelfTmbKey.Contains( "[SKL_ID]" ) ) {
                    SelfTmbKey = string.Empty;
                    return;
                }
            }
            else SelfTmbKey = forceSelfKey; // Manually specified key

            if( justSelf ) return;

            CastVfxKey = action.VFX?.Value?.VFX.Value?.Location;

            // split this off into its own item
            var hitKey = action.ActionTimelineHit?.Value?.Key.ToString();
            if( hitKey.Contains( "normal_hit" ) ) hitKey = string.Empty;
            if( string.IsNullOrEmpty( hitKey ) ) return;
            var hitAction = new Lumina.Excel.GeneratedSheets.Action {
                Icon = action.Icon,
                Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) ),
                IsPlayerAction = action.IsPlayerAction,
                RowId = action.RowId,
                AnimationEnd = action.ActionTimelineHit
            };
            HitAction = new XivActionNonPlayer( hitAction, justSelf: true );
        }
    }
}
