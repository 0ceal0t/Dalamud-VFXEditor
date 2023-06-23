using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VfxEditor.Select {
    public class RaceStruct {
        public readonly string SkeletonId;
        public readonly int HairOffset;

        public RaceStruct( string skeletonId, int hairOffset ) {
            SkeletonId = skeletonId;
            HairOffset = hairOffset;
        }
    }

    public partial class SelectUtils {
        public static string BnpcPath => Path.Combine( Plugin.RootLocation, "Files", "bnpc.json" );
        public static string NpcFilesPath => Path.Combine( Plugin.RootLocation, "Files", "npc_files.json" );
        public static string MiscVfxPath => Path.Combine( Plugin.RootLocation, "Files", "vfx_misc.txt" );
        public static string MiscTmbPath => Path.Combine( Plugin.RootLocation, "Files", "tmb_misc.txt" );
        public static string MiscUldPath => Path.Combine( Plugin.RootLocation, "Files", "uld_misc.txt" );

        [GeneratedRegex( "\\u0000([a-zA-Z0-9\\/_]*?)\\.avfx", RegexOptions.Compiled )]
        private static partial Regex AvfxRegexPattern();
        public static readonly Regex AvfxRegex = AvfxRegexPattern();

        // https://github.com/imchillin/CMTool/blob/master/ConceptMatrix/Views/SpecialControl.xaml.cs#L365

        public static readonly Dictionary<string, RaceStruct> RaceAnimationIds = new() {
            { "Midlander M", new RaceStruct( "c0101", 0 ) },
            { "Midlander F", new RaceStruct( "c0201", 100 ) },
            { "Highlander M", new RaceStruct( "c0301", 200 ) },
            { "Highlander F", new RaceStruct( "c0401", 300 ) },
            { "Elezen M", new RaceStruct( "c0501", 400 ) },
            { "Elezen F", new RaceStruct( "c0601", 500 ) },
            { "Miquote M", new RaceStruct( "c0701", 800 ) },
            { "Miquote F", new RaceStruct( "c0801", 900 ) },
            { "Roegadyn M", new RaceStruct( "c0901", 1000 ) },
            { "Roegadyn F", new RaceStruct( "c1001", 1100 ) },
            { "Lalafell M", new RaceStruct( "c1101", 600 ) },
            { "Lalafell F", new RaceStruct( "c1201", 700 ) },
            { "Aura M", new RaceStruct( "c1301", 1200 ) },
            { "Aura F", new RaceStruct( "c1401", 1300 ) },
            { "Hrothgar M", new RaceStruct( "c1501", 1400 ) },
            // 1601 coming soon (tm)
            { "Viera M", new RaceStruct( "c1701", 1600 ) },
            { "Viera F", new RaceStruct( "c1801", 1700 ) },
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

        public static readonly int MaxFaces = 4;

        public static readonly int HairEntries = 100;

        public static Dictionary<string, string> FileExistsFilter( Dictionary<string, string> dict ) =>
            dict.Where( x => Plugin.DataManager.FileExists( x.Value ) ).ToDictionary( x => x.Key, x => x.Value );

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

        public static Dictionary<string, string> GetAllFacePaps( string modelId, string path ) {
            Dictionary<string, string> ret = new();
            for( var face = 1; face <= MaxFaces; face++ ) {
                ret.Add( $"Face {face}", $"chara/human/{modelId}/animation/f{face:D4}/nonresident/{path}.pap" );
            }
            return FileExistsFilter( ret );
        }

        public static Dictionary<string, Dictionary<string, string>> GetAllFacePaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, Dictionary<string, string>>();
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetAllFacePaps( x.Value.SkeletonId, path ) );
        }
    }
}
