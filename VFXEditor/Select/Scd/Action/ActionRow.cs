namespace VfxEditor.Select.Scd.Action {
    public class ActionRow {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public string StartTmbPath => SelectDataUtils.ToTmbPath( StartTmbKey );
        public string EndTmbPath => SelectDataUtils.ToTmbPath( EndTmbKey );
        public string HitTmbPath => SelectDataUtils.ToTmbPath( HitTmbKey );
        public string CastVfxPath => SelectDataUtils.ToVfxPath( CastVfxKey );
        public string StartVfxPath => SelectDataUtils.ToVfxPath( StartVfxKey );

        private readonly string StartTmbKey;
        private readonly string EndTmbKey;
        private readonly string HitTmbKey;
        private readonly string CastVfxKey;
        private readonly string StartVfxKey;

        public ActionRow( Lumina.Excel.GeneratedSheets.Action action ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            StartTmbKey = action.AnimationStart.Value?.Name.Value?.Key.ToString();
            EndTmbKey = action.AnimationEnd.Value?.Key.ToString();
            HitTmbKey = action.ActionTimelineHit.Value?.Key.ToString();
            StartVfxKey = action.AnimationStart.Value?.VFX.Value?.Location.ToString();
            CastVfxKey = action.VFX.Value?.VFX.Value?.Location.ToString();
        }
    }
}
