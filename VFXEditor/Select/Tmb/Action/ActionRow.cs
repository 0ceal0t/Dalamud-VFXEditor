using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Tmb.Action {
    public readonly struct ActionTmbData {
        public readonly string Path;
        public readonly bool IsMotionDisabled;

        public ActionTmbData( ActionTimeline timeline ) {
            IsMotionDisabled = timeline?.IsMotionCanceledByMoving ?? false;
            Path = ToTmb( timeline?.Key.ToString() );
        }

        public ActionTmbData( WeaponTimeline timeline ) {
            IsMotionDisabled = false;
            Path = ToTmb( timeline?.File.ToString() );
        }

        private static string ToTmb( string key ) {
            if( string.IsNullOrEmpty( key ) ) return "";
            if( key.Contains( "[SKL_ID]" ) ) return "";
            return $"chara/action/{key}.tmb";
        }
    }

    public class ActionRow {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        public readonly ActionTmbData Start;
        public readonly ActionTmbData End;
        public readonly ActionTmbData Hit;
        public readonly ActionTmbData Weapon;

        public ActionRow( Lumina.Excel.GeneratedSheets.Action action ) {
            RowId = ( int )action.RowId;
            Icon = action.Icon;
            Name = action.Name.ToString();

            Start = new ActionTmbData( action.AnimationStart.Value?.Name.Value );
            End = new ActionTmbData( action.AnimationEnd.Value );
            Hit = new ActionTmbData( action.ActionTimelineHit.Value );
            Weapon = new ActionTmbData( action.AnimationEnd.Value?.WeaponTimeline.Value );
        }
    }
}
