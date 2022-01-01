namespace VFXSelect.Select.Rows {
    public class XivTmb {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        public readonly string StartTmb;
        public readonly string EndTmb;
        public readonly string HitTmb;
        public readonly string WeaponTmb;

        public XivTmb( Lumina.Excel.GeneratedSheets.Action action ) {
            RowId = ( int )action.RowId;
            Icon = action.Icon;
            Name = action.Name.ToString();

            var startKey = action.AnimationStart?.Value?.Name?.Value?.Key.ToString();
            var endKey = action.AnimationEnd?.Value?.Key.ToString();
            var hitKey = action.ActionTimelineHit?.Value?.Key.ToString();
            var weaponKey = action.AnimationEnd?.Value?.WeaponTimeline?.Value?.File.ToString();

            StartTmb = ToTmb( startKey );
            EndTmb = ToTmb( endKey );
            HitTmb = ToTmb( hitKey );
            WeaponTmb = ToTmb( weaponKey );
        }

        private static string ToTmb( string key ) {
            if( string.IsNullOrEmpty( key ) ) return "";
            if( key.Contains( "[SKL_ID]" ) ) return "";
            return $"chara/action/{key}.tmb";
        }
    }
}
