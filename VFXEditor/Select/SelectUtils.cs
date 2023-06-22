using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VfxEditor.Select {
    public partial class SelectUtils {
        public static string BnpcPath => Path.Combine( Plugin.RootLocation, "Files", "bnpc.json" );
        public static string NpcFilesPath => Path.Combine( Plugin.RootLocation, "Files", "npc_files.json" );
        public static string MiscVfxPath => Path.Combine( Plugin.RootLocation, "Files", "vfx_misc.txt" );
        public static string MiscTmbPath => Path.Combine( Plugin.RootLocation, "Files", "tmb_misc.txt" );
        public static string MiscUldPath => Path.Combine( Plugin.RootLocation, "Files", "uld_misc.txt" );

        [GeneratedRegex( "\\u0000([a-zA-Z0-9\\/_]*?)\\.avfx", RegexOptions.Compiled )]
        private static partial Regex AvfxRegexPattern();
        public static readonly Regex AvfxRegex = AvfxRegexPattern();

        public struct RaceStruct {
            public string SkeletonId;
            public int MinFace;
            public int MaxFace;

            public RaceStruct( string skeletonId, int minFace, int maxFace ) {
                SkeletonId = skeletonId;
                MinFace = minFace;
                MaxFace = maxFace;
            }
        }

        public static readonly Dictionary<string, RaceStruct> RaceAnimationIds = new() {
            { "Midlander M", new RaceStruct("c0101", 1, 4) },
            { "Midlander F", new RaceStruct("c0201", 1, 4) },
            { "Highlander M", new RaceStruct("c0301", 1, 4) },
            { "Highlander F", new RaceStruct("c0401", 1, 4) },
            { "Elezen M", new RaceStruct("c0501", 1, 4) },
            { "Elezen F", new RaceStruct("c0601", 1, 4) },
            { "Miquote M", new RaceStruct("c0701", 1, 4) },
            { "Miquote F", new RaceStruct("c0801", 1, 4) },
            { "Roegadyn M", new RaceStruct("c0901", 1, 4) },
            { "Roegadyn F", new RaceStruct("c1001", 1, 4) },
            { "Lalafell M", new RaceStruct("c1101", 1, 4) },
            { "Lalafell F", new RaceStruct("c1201", 1, 4) },
            { "Aura M", new RaceStruct("c1301", 1, 4) },
            { "Aura F", new RaceStruct("c1401", 1, 4) },
            { "Hrothgar M", new RaceStruct("c1501", 1, 4) },
            // 1601 coming soon (tm)
            { "Viera M", new RaceStruct("c1701", 1, 4) },
            { "Viera F", new RaceStruct("c1801", 1, 4) },
        };

        public static readonly Dictionary<string, string> JobAnimationIds = new() {
            { "Warrior", "bt_2ax_emp" },
            { "Paladin", "bt_swd_sld" },
            { "Gunbreaker", "bt_2gb_emp" },
            { "Dark Knight", "bt_2sw_emp" },
            { "Astrologian", "bt_2gl_emp" },
            { "Sage", "bt_2ff_emp" },
            { "Scholar", "bt_2bk_emp" },
            { "White Mage", "bt_stf_sld" },
            { "Machinist", "bt_2gn_emp" },
            { "Dancer", "bt_chk_chk" },
            { "Bard", "bt_2bw_emp" },
            { "Samurai", "bt_2kt_emp" },
            { "Dragoon", "bt_2sp_emp" },
            { "Monk", "bt_clw_clw" },
            { "Ninja", "bt_dgr_dgr" },
            { "Reaper", "bt_2km_emp" },
            { "Red Mage", "bt_2rp_emp" },
            { "Black Mage", "bt_jst_sld" },
            { "Summoner", "bt_2bk_emp" },
            { "Blue Mage", "bt_rod_emp" },
        };

        public static readonly Dictionary<string, string> JobMovementOverride = new() {
            { "Black Mage", "bt_stf_sld" },
            { "Ninja", "bt_nin_nin" },
        };

        public static readonly Dictionary<string, string> JobDrawOverride = new() {
            { "Black Mage", "bt_stf_sld" }
        };

        public static readonly Dictionary<string, string> JobAutoOverride = new() {
            { "Black Mage", "bt_stf_sld" }
        };

        public static readonly int MaxChangePoses = 6;

        public static Dictionary<string, string> FileExistsFilter( Dictionary<string, string> dict ) => dict.Where( x => Plugin.DataManager.FileExists( x.Value ) ).ToDictionary( x => x.Key, x => x.Value );

        public static string GetSkeletonPath( string skeletonId, string path ) => $"chara/human/{skeletonId}/animation/a0001/{path}";

        public static Dictionary<string, string> GetAllSkeletonPaths( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, string>();
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetSkeletonPath( x.Value.SkeletonId, path ) );
        }

        public static Dictionary<string, string> GetAllJobPaps( string jobId, string path ) => FileExistsFilter( GetAllSkeletonPaths( $"{jobId}/{path}.pap" ) );

        public static Dictionary<string, Dictionary<string, string>> GetAllJobPaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, Dictionary<string, string>>();
            return JobAnimationIds.ToDictionary( x => x.Key, x => GetAllJobPaps( x.Value, path ) );
        }

        public static Dictionary<string, string> GetAllFacePaps( string modelId, string path, int minFace, int maxFace ) {
            Dictionary<string, string> ret = new();
            for( var face = minFace; face <= maxFace; face++ ) {
                ret.Add( $"Face {face}", $"chara/human/{modelId}/animation/f{face:D4}/nonresident/{path}.pap" );
            }
            return FileExistsFilter( ret );
        }

        public static Dictionary<string, Dictionary<string, string>> GetAllFacePaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, Dictionary<string, string>>();
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetAllFacePaps( x.Value.SkeletonId, path, x.Value.MinFace, x.Value.MaxFace ) );
        }
    }
}
