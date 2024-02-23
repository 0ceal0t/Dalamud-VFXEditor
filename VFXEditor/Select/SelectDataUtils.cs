using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VfxEditor.Select.Data;

namespace VfxEditor.Select {
    public partial class SelectDataUtils {
        public static string BnpcPath => Path.Combine( Plugin.RootLocation, "Files", "bnpc.json" );
        public static string NpcFilesPath => Path.Combine( Plugin.RootLocation, "Files", "npc_files.json" );
        public static string CommonVfxPath => Path.Combine( Plugin.RootLocation, "Files", "common_vfx" );
        public static string CommonTmbPath => Path.Combine( Plugin.RootLocation, "Files", "common_tmb" );
        public static string CommonUldPath => Path.Combine( Plugin.RootLocation, "Files", "common_uld" );
        public static string CommonShpkPath => Path.Combine( Plugin.RootLocation, "Files", "common_shpk" );
        public static string CommonShcdPath => Path.Combine( Plugin.RootLocation, "Files", "common_shcd" );
        public static string CommonRacialPath => Path.Combine( Plugin.RootLocation, "Files", "common_racial" );

        [GeneratedRegex( "\\u0000([a-zA-Z0-9\\/_]*?)\\.avfx", RegexOptions.Compiled )]
        private static partial Regex AvfxRegexPattern();
        public static readonly Regex AvfxRegex = AvfxRegexPattern();

        [GeneratedRegex( "\\u0000([a-zA-Z0-9\\/_]*?)\\.scd", RegexOptions.Compiled )]
        private static partial Regex ScdRegexPattern();
        public static readonly Regex ScdRegex = ScdRegexPattern();

        // https://github.com/imchillin/CMTool/blob/master/ConceptMatrix/Views/SpecialControl.xaml.cs#L365

        public static readonly List<RacialData> CharacterRaces = [
            new RacialData( "Midlander M", "c0101", 1 ),
            new RacialData( "Midlander F","c0201", 101 ),
            new RacialData( "Highlander M", "c0301", 201 ),
            new RacialData( "Highlander F", "c0401", 301 ),
            new RacialData( "Elezen M", "c0501", 401 ),
            new RacialData( "Elezen F", "c0601", 501 ),
            new RacialData( "Miquote M", "c0701", 801 ),
            new RacialData( "Miquote F", "c0801", 901 ),
            new RacialData( "Roegadyn M", "c0901", 1001 ),
            new RacialData( "Roegadyn F", "c1001", 1101 ),
            new RacialData( "Lalafell M", "c1101",601 ),
            new RacialData( "Lalafell F", "c1201", 701 ),
            new RacialData( "Aura M", "c1301", 1201 ),
            new RacialData( "Aura F", "c1401", 1301 ),
            new RacialData( "Hrothgar M", "c1501", 1401 ),
            // 1601 coming soon (tm) - 25
            new RacialData( "Viera M", "c1701", 1601 ),
            new RacialData( "Viera F", "c1801", 1701 )
        ];

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

        public static Dictionary<string, string> FileExistsFilter( Dictionary<string, string> dict ) => dict.Where( x => Dalamud.DataManager.FileExists( x.Value ) ).ToDictionary( x => x.Key, x => x.Value );

        public static string GetSkeletonPath( string skeletonId, string path ) => $"chara/human/{skeletonId}/animation/a0001/{path}";

        public static Dictionary<string, string> GetAllSkeletonPaths( string path ) {
            if( string.IsNullOrEmpty( path ) ) return [];
            return CharacterRaces.ToDictionary( x => x.Name, x => GetSkeletonPath( x.Id, path ) );
        }

        public static Dictionary<string, string> GetAllJobPaps( string jobId, string path ) => FileExistsFilter( GetAllSkeletonPaths( $"{jobId}/{path}.pap" ) );

        public static Dictionary<string, Dictionary<string, string>> GetAllJobPaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return [];
            return JobAnimationIds.ToDictionary( x => x.Key, x => GetAllJobPaps( x.Value, path ) );
        }

        public static Dictionary<string, Dictionary<string, string>> GetAllFacePaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return [];
            Dictionary<string, Dictionary<string, string>> ret = [];
            foreach( var item in CharacterRaces ) {
                ret[item.Name] = item.FaceOptions.ToDictionary( x => $"Face {x}", x => $"chara/human/{item.Id}/animation/f{x:D4}/nonresident/{path}.pap" );
            }
            return ret;
        }

        public static string ToTmbPath( string key ) => ( string.IsNullOrEmpty( key ) || key.Contains( "[SKL_ID]" ) ) ? string.Empty : $"chara/action/{key}.tmb";

        public static string ToVfxPath( string key ) => string.IsNullOrEmpty( key ) ? string.Empty : $"vfx/common/eff/{key}.avfx";
    }
}
