using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivActionNonPlayer : XivActionBase
    {
        public bool IsPlaceholder = false;
        public List<XivActionNonPlayer> PlaceholderActions;

        public XivActionNonPlayer( Lumina.Excel.GeneratedSheets.Action action, bool justSelf = false, string forceSelfKey = "" )
        {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            if( forceSelfKey == "" ) {
                SelfVFXKey = action.AnimationEnd?.Value?.Key.ToString();
                SelfVFXExists = !string.IsNullOrEmpty( SelfVFXKey );
                if( SelfVFXExists ) {
                    var selfMKey = new MonsterKey( SelfVFXKey );
                    if( selfMKey.isMonster && selfMKey.skeletonKey == "[SKL_ID]" ) {
                        IsPlaceholder = true;
                        return;
                    }
                }
            }
            else // manually specified key
            {
                SelfVFXExists = true;
                SelfVFXKey = forceSelfKey;
            }

            if( !justSelf ) // when handling a hit vfx
            {
                CastVFX = action.VFX?.Value?.VFX.Value?.Location;
                CastVFXExists = !string.IsNullOrEmpty( CastVFX );

                // split this off into its own item
                HitVFXKey = action.ActionTimelineHit?.Value?.Key.ToString();
                HitVFXExists = !string.IsNullOrEmpty( HitVFXKey ) && !HitVFXKey.Contains("normal_hit");
                if( HitVFXExists )
                {
                    var sAction = new Lumina.Excel.GeneratedSheets.Action();
                    sAction.Icon = action.Icon;
                    sAction.Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) );
                    sAction.IsPlayerAction = action.IsPlayerAction;
                    sAction.RowId = action.RowId;
                    sAction.AnimationEnd = action.ActionTimelineHit;
                    HitAction = new XivActionNonPlayer( sAction, justSelf: true);
                }
            }

            VfxExists = !IsPlaceholder && ( CastVFXExists || SelfVFXExists );
        }
    }

    public struct MonsterKey
    {
        public static Regex rx = new Regex( @"mon_sp\/(.*?)\/(.*)", RegexOptions.Compiled );

        public bool isMonster;
        public string skeletonKey;
        public string actionId;

        public MonsterKey(string key )
        {
            Match match = rx.Match( key );
            if( match.Success )
            {
                isMonster = true;
                skeletonKey = match.Groups[1].Value;
                actionId = match.Groups[2].Value;
            }
            else
            {
                isMonster = false;
                skeletonKey = "";
                actionId = "";
            }
        }
    }
}
