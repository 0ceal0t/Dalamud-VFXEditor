using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor
{
    public class XivAction : XivActionBase
    {
        public XivAction( Lumina.Excel.GeneratedSheets.Action action, bool justSelf = false)
        {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;

            SelfVFXKey = action.AnimationEnd?.Value?.Key.ToString();
            SelfVFXExists = !string.IsNullOrEmpty( SelfVFXKey );

            if( !justSelf )
            {
                CastVFX = action.VFX?.Value?.VFX.Value?.Location;
                CastVFXExists = !string.IsNullOrEmpty( CastVFX );

                //startVfx = action.AnimationStart.Value?.VFX.Value?.Location;

                // split this off into its own item
                HitVFXKey = action.ActionTimelineHit?.Value?.Key.ToString();
                HitVFXExists = !string.IsNullOrEmpty( HitVFXKey );
                if( HitVFXExists )
                {
                    var sAction = new Lumina.Excel.GeneratedSheets.Action();
                    sAction.Name = new Lumina.Text.SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) );
                    sAction.IsPlayerAction = action.IsPlayerAction;
                    sAction.RowId = action.RowId;
                    sAction.AnimationEnd = action.ActionTimelineHit;
                    HitAction = new XivAction( sAction, justSelf:true );
                }
            }

            VfxExists = ( CastVFXExists || SelfVFXExists);
        }
    }
}
