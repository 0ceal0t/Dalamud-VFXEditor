using Lumina.Excel.GeneratedSheets;
using Lumina.Text;
using System.Text;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionRowVfx {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        private readonly string TmbKey;
        private readonly string CastVfxKey;
        private readonly string StartVfxKey;

        public readonly ActionRowVfx HitAction;

        public string TmbPath => SelectDataUtils.ToTmbPath( TmbKey );
        public string CastVfxPath => SelectDataUtils.ToVfxPath( CastVfxKey );
        public string StartVfxPath => SelectDataUtils.ToVfxPath( StartVfxKey );

        public ActionRowVfx( Action action, bool isHit ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            TmbKey = action.AnimationEnd.Value?.Key.ToString();

            if( isHit ) {
                HitAction = null;
                return;
            }

            StartVfxKey = action.AnimationStart.Value?.VFX.Value?.Location;
            CastVfxKey = action.VFX.Value?.VFX.Value?.Location;

            var hitKey = action.ActionTimelineHit?.Value?.Key.ToString();
            if( hitKey.Contains( "normal_hit" ) ) hitKey = string.Empty;
            if( string.IsNullOrEmpty( hitKey ) ) {
                HitAction = null;
                return;
            }

            if( string.IsNullOrEmpty( hitKey ) ) return;
            var hitAction = new Action {
                Icon = action.Icon,
                Name = new SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) ),
                IsPlayerAction = action.IsPlayerAction,
                RowId = action.RowId,
                AnimationEnd = action.ActionTimelineHit
            };
            HitAction = new ActionRowVfx( hitAction, true );
        }
    }
}