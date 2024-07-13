using Lumina.Excel.GeneratedSheets;
using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Actions {
    public class ActionRowPap : ISelectItemWithIcon {
        public readonly string Name;
        public readonly int RowId;
        public readonly ushort Icon;

        public readonly string StartPath;
        public readonly string EndPath;
        public readonly string HitPath;

        public ActionRowPap( Action action ) {
            Name = action.Name.ToString();
            RowId = ( int )action.RowId;
            Icon = action.Icon;

            StartPath = ToPap( action.AnimationStart.Value?.Name.Value );
            EndPath = ToPap( action.AnimationEnd.Value );
            HitPath = ToPap( action.ActionTimelineHit.Value );
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
            return $"{loadType}/{key}";
        }

        public string GetName() => Name;

        public uint GetIconId() => Icon;
    }
}