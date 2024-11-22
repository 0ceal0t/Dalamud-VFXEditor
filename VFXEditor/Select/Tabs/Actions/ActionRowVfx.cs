using Lumina.Excel;
using Lumina.Excel.Sheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionRowVfx : ISelectItemWithIcon {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public readonly string TmbPath;
        public readonly string CastVfxPath;
        public readonly string StartVfxPath;

        public readonly ActionRowVfx HitAction;

        public ActionRowVfx( string name, int rowId, ushort icon, RowRef<ActionTimeline> timeline ) {
            Name = name;
            RowId = rowId;
            Icon = icon;

            TmbPath = ActionRow.ToTmbPath( timeline.ValueNullable?.Key.ToString() );
        }

        public ActionRowVfx( Action action ) : this( action.Name.ToString(), ( int )action.RowId, action.Icon, action.AnimationEnd ) {
            StartVfxPath = ActionRow.ToVfxPath( action.AnimationStart.ValueNullable?.VFX.ValueNullable?.Location.ExtractText() );
            CastVfxPath = ActionRow.ToVfxPath( action.VFX.ValueNullable?.VFX.ValueNullable?.Location.ExtractText() );

            var hitKey = action.ActionTimelineHit.ValueNullable?.Key.ToString();
            if( string.IsNullOrEmpty( hitKey ) || hitKey.Contains( "normal_hit" ) ) return;
            HitAction = new ActionRowVfx( $"{Name} (Target)", RowId, Icon, action.ActionTimelineHit );
        }

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}