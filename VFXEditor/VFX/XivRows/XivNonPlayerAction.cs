using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXEditor
{
    public class XivNonPlayerAction : XivActionBase
    {
        public bool IsPlaceholder = false;
        public List<XivNonPlayerAction> PlaceholderActions;

        public XivNonPlayerAction( Lumina.Excel.GeneratedSheets.Action action, Dictionary<string, List<string>> dict, bool justSelf = false, string forceSelfKey = "" )
        {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;

            if( forceSelfKey == "" )
            {
                SelfVFXKey = action.AnimationEnd?.Value?.Key.ToString();
                SelfVFXExists = !string.IsNullOrEmpty( SelfVFXKey );
                if( SelfVFXExists )
                {
                    var selfMKey = new MonsterKey( SelfVFXKey );
                    if( selfMKey.isMonster && selfMKey.skeletonKey == "[SKL_ID]" ) // this is a placeholder, need to get all of the actual values
                    {

                        SelfVFXExists = false;
                        IsPlaceholder = true;
                        PlaceholderActions = new List<XivNonPlayerAction>();

                        if( dict.ContainsKey( selfMKey.actionId ) )
                        {
                            foreach(var sklId in dict[selfMKey.actionId] )
                            {
                                var sAction = new Lumina.Excel.GeneratedSheets.Action();
                                sAction.Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " - " + sklId ) );
                                sAction.IsPlayerAction = action.IsPlayerAction;
                                sAction.RowId = action.RowId;
                                sAction.VFX = action.VFX; // preserve cast vfx

                                string actualKey = "mon_sp/" + sklId + "/" + selfMKey.actionId;

                                PlaceholderActions.Add( new XivNonPlayerAction( sAction, dict, forceSelfKey: actualKey, justSelf: justSelf) );
                            }
                        }
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
                    sAction.Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) );
                    sAction.IsPlayerAction = action.IsPlayerAction;
                    sAction.RowId = action.RowId;
                    sAction.AnimationEnd = action.ActionTimelineHit;
                    HitAction = new XivNonPlayerAction( sAction, dict, justSelf: true);
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
