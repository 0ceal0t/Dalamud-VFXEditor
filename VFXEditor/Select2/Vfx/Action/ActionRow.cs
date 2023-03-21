using Lumina.Text;
using System.Text;

namespace VfxEditor.Select2.Vfx.Action {
    public class ActionRow {
        public static readonly string CastPrefix = "vfx/common/eff/";

        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public readonly string SelfTmbKey;
        public readonly ActionRow HitAction;
        public readonly string CastVfxKey;

        public string CastVfxPath => string.IsNullOrEmpty( CastVfxKey ) ? "" : $"{CastPrefix}{CastVfxKey}.avfx";
        public bool HasVfx => !string.IsNullOrEmpty( CastVfxKey ) || !string.IsNullOrEmpty( SelfTmbKey );
        public string TmbPath => $"chara/action/{SelfTmbKey}.tmb";

        public ActionRow( Lumina.Excel.GeneratedSheets.Action action, bool justSelf ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            var selfKey = action.AnimationEnd.Value?.Key.ToString();
            SelfTmbKey = ( string.IsNullOrEmpty( selfKey ) || selfKey.Contains( "[SKL_ID]" ) ) ? string.Empty : selfKey;

            if( justSelf ) {
                HitAction = null;
                return;
            }

            CastVfxKey = action.VFX.Value?.VFX.Value?.Location;

            var hitKey = action.ActionTimelineHit?.Value?.Key.ToString();
            if( hitKey.Contains( "normal_hit" ) ) hitKey = string.Empty;
            if( string.IsNullOrEmpty( hitKey ) ) {
                HitAction = null;
                return;
            }

            if( string.IsNullOrEmpty( hitKey ) ) return;
            var hitAction = new Lumina.Excel.GeneratedSheets.Action {
                Icon = action.Icon,
                Name = new SeString( Encoding.UTF8.GetBytes( Name + " / Target" ) ),
                IsPlayerAction = action.IsPlayerAction,
                RowId = action.RowId,
                AnimationEnd = action.ActionTimelineHit
            };
            HitAction = new ActionRow( hitAction, justSelf: true );
        }
    }
}
