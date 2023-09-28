namespace VfxEditor.Select.Tmb.Action {
    public class ActionRow {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        private readonly string StartKey;
        private readonly string EndKey;
        private readonly string HitKey;
        private readonly string WeaponKey;

        public readonly bool StartMotion;
        public readonly bool EndMotion;
        public readonly bool HitMotion;

        public string StartPath => SelectDataUtils.ToTmbPath( StartKey );
        public string EndPath => SelectDataUtils.ToTmbPath( EndKey );
        public string HitPath => SelectDataUtils.ToTmbPath( HitKey );
        public string WeaponPath => SelectDataUtils.ToTmbPath( WeaponKey );

        public ActionRow( Lumina.Excel.GeneratedSheets.Action action ) {
            RowId = ( int )action.RowId;
            Icon = action.Icon;
            Name = action.Name.ToString();

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
    }
}
