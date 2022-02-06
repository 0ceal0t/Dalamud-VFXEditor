using System.Collections.Generic;

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
                return $"{weapon}/{key}.pap";
            }
            if (loadType == 1) {
                return $"bt_common/{key}.pap";
            }
            if (loadType == 0) {
                return $"bt_common/resident/action.pap";
            }
            return "";
        }

        private static string GenerateSkeleton( string skeletonId, string path ) => $"chara/human/{skeletonId}/animation/a0001/{path}";

        public static Dictionary<string, string> GenerateAllSkeletons( string path ) {
            Dictionary<string, string> ret = new();
            if( string.IsNullOrEmpty( path ) ) return ret;

            ret.Add( "Midlander M", GenerateSkeleton( "c0101", path) );
            ret.Add( "Midlander F", GenerateSkeleton( "c0201", path ) );
            ret.Add( "Highlander M", GenerateSkeleton( "c0301", path ) );
            ret.Add( "Highlander F", GenerateSkeleton( "c0401", path ) );
            ret.Add( "Elezen M", GenerateSkeleton( "c0501", path ) );
            ret.Add( "Elezen F", GenerateSkeleton( "c0601", path ) );
            ret.Add( "Miquote M", GenerateSkeleton( "c0701", path ) );
            ret.Add( "Miquote F", GenerateSkeleton( "c0801", path ) );
            ret.Add( "Roegadyn M", GenerateSkeleton( "c0901", path ) );
            ret.Add( "Roegadyn F", GenerateSkeleton( "c1001", path ) );
            ret.Add( "Lalafell M", GenerateSkeleton( "c1101", path ) );
            ret.Add( "Lalafell F", GenerateSkeleton( "c1201", path ) );
            ret.Add( "AuRa M", GenerateSkeleton( "c1301", path ) );
            ret.Add( "AuRa F", GenerateSkeleton( "c1401", path ) );
            ret.Add( "Hrothgar M", GenerateSkeleton( "c1501", path ) );
            // 1601 coming soon (tm)
            ret.Add( "Viera M", GenerateSkeleton( "c1701", path ) );
            ret.Add( "Viera F", GenerateSkeleton( "c1801", path ) );

            return ret;
        }
    }
}
