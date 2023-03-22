using Lumina.Excel.GeneratedSheets;

namespace VfxEditor.Select.Pap.Action {
    public class ActionRow {
        public readonly ushort Icon;
        public readonly int RowId;
        public readonly string Name;

        public readonly string Start;
        public readonly string End;
        public readonly string Hit;

        public ActionRow( Lumina.Excel.GeneratedSheets.Action action ) {
            RowId = ( int )action.RowId;
            Icon = action.Icon;
            Name = action.Name.ToString();

            Start = ToPap( action.AnimationStart.Value?.Name.Value );
            End = ToPap( action.AnimationEnd.Value );
            Hit = ToPap( action.ActionTimelineHit.Value );
        }

        // chara/human/c0101/animation/a0001/bt_common/ability/2bw_bard/abl001.pap
        // chara/human/c0101/animation/a0001/bt_2bw_emp/ws/bt_2bw_emp/ws_s01.pap
        // chara/human/c0101/animation/a0001/bt_common/magic/2rp_redmage/mgc012.pap
        // chara/human/c0101/animation/a0001/bt_common/rol_common/rol021.pap
        // chara/human/c0101/animation/a0001/bt_common/resident/action.pap
        // chara/human/c0101/animation/a0001/bt_common/limitbreak/lbk_dancer_start.pap
        // timline -> loadType = 1 (not in action.pap)

        private static string ToPap( ActionTimeline timeline ) {
            if( timeline == null ) return "";
            var key = timeline?.Key.ToString();
            if( string.IsNullOrEmpty( key ) ) return "";
            if( key.Contains( "[SKL_ID]" ) ) return "";

            var loadType = timeline.LoadType;
            if( loadType == 2 && key.StartsWith( "ws" ) ) {
                // human_sp/c0501/human_sp103
                // emote/b_pose01_loop
                // ws/bt_2sw_emp/ws_s02
                var split = key.Split( '/' );
                var weapon = split[1];
                return $"{weapon}/{key}.pap";
            }
            else if( loadType == 1 ) return $"bt_common/{key}.pap";
            else if( loadType == 0 ) return $"bt_common/resident/action.pap";
            return "";
        }
    }
}
