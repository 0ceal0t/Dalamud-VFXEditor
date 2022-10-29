using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Rows {
    public class XivActionNonPlayer : XivActionBase {
        public bool IsPlaceholder = false;
        public List<XivActionNonPlayer> PlaceholderActions;

        public XivActionNonPlayer( Lumina.Excel.GeneratedSheets.Action action, bool justSelf = false, string forceSelfKey = "" ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            if( forceSelfKey == "" ) {
                SelfKey = action.AnimationEnd?.Value?.Key.ToString();
                SelfKeyExists = !string.IsNullOrEmpty( SelfKey );
                if( SelfKeyExists ) {
                    var selfMKey = new MonsterKey( SelfKey );
                    if( selfMKey.isMonster && selfMKey.skeletonKey == "[SKL_ID]" ) {
                        IsPlaceholder = true;
                        return;
                    }
                }
            }
            else { // Manually specified key
                SelfKeyExists = true;
                SelfKey = forceSelfKey;
            }

            if( !justSelf ) { // Just loading hit
                Castvfx = action.VFX?.Value?.VFX.Value?.Location;
                CastKeyExists = !string.IsNullOrEmpty( Castvfx );

                // split this off into its own item
                HitKey = action.ActionTimelineHit?.Value?.Key.ToString();
                HitKeyExists = !string.IsNullOrEmpty( HitKey ) && !HitKey.Contains( "normal_hit" );
                if( HitKeyExists ) {
                    var sAction = new Lumina.Excel.GeneratedSheets.Action {
                        Icon = action.Icon,
                        Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) ),
                        IsPlayerAction = action.IsPlayerAction,
                        RowId = action.RowId,
                        AnimationEnd = action.ActionTimelineHit
                    };
                    HitAction = new XivActionNonPlayer( sAction, justSelf: true );
                }
            }

            KeyExists = !IsPlaceholder && ( CastKeyExists || SelfKeyExists );
        }
    }

    public struct MonsterKey {
        public static readonly Regex rx = new( @"mon_sp\/(.*?)\/(.*)", RegexOptions.Compiled );

        public bool isMonster;
        public string skeletonKey;
        public string actionId;

        public MonsterKey( string key ) {
            var match = rx.Match( key );
            if( match.Success ) {
                isMonster = true;
                skeletonKey = match.Groups[1].Value;
                actionId = match.Groups[2].Value;
            }
            else {
                isMonster = false;
                skeletonKey = "";
                actionId = "";
            }
        }
    }
}
