namespace VFXSelect.Select.Rows {
    public class XivPap {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        public readonly string StartPap;
        public readonly string EndPap;
        public readonly string HitPap;

        public XivPap( Lumina.Excel.GeneratedSheets.Action action ) {
            RowId = ( int )action.RowId;
            Icon = action.Icon;
            Name = action.Name.ToString();

            var startKey = action.AnimationStart?.Value?.Name?.Value;
            var endKey = action.AnimationEnd?.Value;
            var hitKey = action.ActionTimelineHit?.Value;

            StartPap = ToPap( startKey );
            EndPap = ToPap( endKey );
            HitPap = ToPap( hitKey );
        }

        public XivPap( int rowId, ushort icon, string name, string startPap, string endPap, string hitPap ) {
            RowId = rowId;
            Icon = icon;
            Name = name;
            StartPap= startPap;
            EndPap= endPap;
            HitPap = hitPap;
        }

        // chara/human/c0101/animation/a0001/bt_common/ability/2bw_bard/abl001.pap
        // chara/human/c0101/animation/a0001/bt_2bw_emp/ws/bt_2bw_emp/ws_s01.pap
        // chara/human/c0101/animation/a0001/bt_common/magic/2rp_redmage/mgc012.pap
        // chara/human/c0101/animation/a0001/bt_common/rol_common/rol021.pap
        // chara/human/c0101/animation/a0001/bt_common/resident/action.pap
        // chara/human/c0101/animation/a0001/bt_common/limitbreak/lbk_dancer_start.pap
        // timline -> loadType = 1 (not in action.pap)

        private static string ToPap( Lumina.Excel.GeneratedSheets.ActionTimeline timeline ) {
            if( timeline == null ) return "";
            var key = timeline?.Key.ToString();
            if( string.IsNullOrEmpty( key ) ) return "";
            if( key.Contains( "[SKL_ID]" ) ) return "";

            var loadType = timeline.LoadType;
            if (loadType == 2 && key.StartsWith("ws")) {
                // human_sp/c0501/human_sp103
                // emote/b_pose01_loop
                // ws/bt_2sw_emp/ws_s02
                var split = key.Split( '/' );
                var weapon = split[1];
                return $"chara/human/c0101/animation/a0001/{weapon}/{key}.pap";
            }
            if (loadType == 1) {
                return $"chara/human/c0101/animation/a0001/bt_common/{key}.pap";
            }
            if (loadType == 0) {
                return $"chara/human/c0101/animation/a0001/bt_common/resident/action.pap";
            }
            return "";
        }
    }
}
