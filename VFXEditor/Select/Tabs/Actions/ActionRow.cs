using Lumina.Excel.GeneratedSheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionRow : ISelectItemWithIcon {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public readonly string StartKey;
        public readonly string EndKey;
        public readonly string HitKey;
        public readonly string WeaponKey;

        public readonly string CastVfxKey;
        public readonly string StartVfxKey;

        public readonly bool StartMotion;
        public readonly bool EndMotion;
        public readonly bool HitMotion;

        public string StartPath => SelectDataUtils.ToTmbPath( StartKey );
        public string EndPath => SelectDataUtils.ToTmbPath( EndKey );
        public string HitPath => SelectDataUtils.ToTmbPath( HitKey );
        public string WeaponPath => SelectDataUtils.ToTmbPath( WeaponKey );
        public string CastVfxPath => SelectDataUtils.ToVfxPath( CastVfxKey );
        public string StartVfxPath => SelectDataUtils.ToVfxPath( StartVfxKey );

        public ActionRow( Action action ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            StartVfxKey = action.AnimationStart.Value?.VFX.Value?.Location.ToString();
            CastVfxKey = action.VFX.Value?.VFX.Value?.Location.ToString();

            var start = action.AnimationStart.Value?.Name.Value;
            var end = action.AnimationEnd.Value;
            var hit = action.ActionTimelineHit.Value;

            StartKey = start?.Key.ToString();
            EndKey = end?.Key.ToString();
            HitKey = hit?.Key.ToString();
            WeaponKey = action.AnimationEnd.Value?.WeaponTimeline.Value?.File.ToString();

            StartMotion = start?.IsMotionCanceledByMoving ?? false;
            EndMotion = end?.IsMotionCanceledByMoving ?? false;
            HitMotion = hit?.IsMotionCanceledByMoving ?? false;
        }

        public string GetName() => Name;
        public uint GetIconId() => Icon;
    }
}