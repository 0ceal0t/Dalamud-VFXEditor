using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VfxEditor.Select {
    public class RacialData {
        public readonly string SkeletonId;

        public RacialData( string skeletonId ) {
            SkeletonId = skeletonId;
        }
    }

    public class RacialOptions {
        public readonly SortedSet<int> Face = [];
        public readonly SortedSet<int> Hair = [];
        public readonly SortedSet<int> Tail = [];
        public readonly SortedSet<int> Ear = [];
        public readonly SortedSet<int> Body = [];
    }

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

        public static Dictionary<string, RacialOptions> RacialOptions {
            get {
                if( OptionsInternal == null ) PopulateRacialOptions();
                return OptionsInternal;
            }
        }

        private static Dictionary<string, RacialOptions> OptionsInternal;

        private static void PopulateRacialOptions() {
            if( OptionsInternal != null ) return;
            OptionsInternal = [];

            // chara/human/c0804/obj/face/f0101/material/mt_c0804f0101_etc_a.mtrl
            // chara/human/c0601/skeleton/face/f0092/skl_c0601f0092.sklb

            var files = File.ReadAllLines( CommonRacialPath );
            foreach( var line in files ) {
                var split = line.Split( "/" );
                var skeleton = split[2]; // c0804
                var type = split[4]; // face
                var code = Convert.ToInt32( split[5][1..] ); // f0101 -> 0101

                if( !OptionsInternal.ContainsKey( skeleton ) ) OptionsInternal[skeleton] = new();
                var data = OptionsInternal[skeleton];

                ( type switch {
                    "face" => data.Face,
                    "hair" => data.Hair,
                    "zear" => data.Ear,
                    "tail" => data.Tail,
                    "body" => data.Body,
                    _ => null
                } )?.Add( code );
            }
        }

        public static readonly Dictionary<string, RacialData> RaceAnimationIds = new() {
            { "Midlander M", new RacialData( "c0101" ) },
            { "Midlander F", new RacialData( "c0201" ) },
            { "Highlander M", new RacialData( "c0301" ) },
            { "Highlander F", new RacialData( "c0401" ) },
            { "Elezen M", new RacialData( "c0501" ) },
            { "Elezen F", new RacialData( "c0601" ) },
            { "Miquote M", new RacialData( "c0701" ) },
            { "Miquote F", new RacialData( "c0801" ) },
            { "Roegadyn M", new RacialData( "c0901" ) },
            { "Roegadyn F", new RacialData( "c1001" ) },
            { "Lalafell M", new RacialData( "c1101" ) },
            { "Lalafell F", new RacialData( "c1201" ) },
            { "Aura M", new RacialData( "c1301" ) },
            { "Aura F", new RacialData( "c1401" ) },
            { "Hrothgar M", new RacialData( "c1501" ) },
            // 1601 coming soon (tm)
            { "Viera M", new RacialData( "c1701" ) },
            { "Viera F", new RacialData( "c1801" ) },
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

        public static Dictionary<string, string> FileExistsFilter( Dictionary<string, string> dict ) => dict.Where( x => Dalamud.DataManager.FileExists( x.Value ) ).ToDictionary( x => x.Key, x => x.Value );

        public static string GetSkeletonPath( string skeletonId, string path ) => $"chara/human/{skeletonId}/animation/a0001/{path}";

        public static Dictionary<string, string> GetAllSkeletonPaths( string path ) {
            if( string.IsNullOrEmpty( path ) ) return [];
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetSkeletonPath( x.Value.SkeletonId, path ) );
        }

        public static Dictionary<string, string> GetAllJobPaps( string jobId, string path ) => FileExistsFilter( GetAllSkeletonPaths( $"{jobId}/{path}.pap" ) );

        public static Dictionary<string, Dictionary<string, string>> GetAllJobPaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return [];
            return JobAnimationIds.ToDictionary( x => x.Key, x => GetAllJobPaps( x.Value, path ) );
        }

        public static Dictionary<string, string> GetAllFacePaps( string modelId, string path ) {
            Dictionary<string, string> ret = [];
            if( RacialOptions.TryGetValue( modelId, out var data ) ) {
                foreach( var face in data.Face ) {
                    ret.Add( $"Face {face}", $"chara/human/{modelId}/animation/f{face:D4}/nonresident/{path}.pap" );
                }
            }

            return FileExistsFilter( ret );
        }

        public static Dictionary<string, Dictionary<string, string>> GetAllFacePaps( string path ) {
            if( string.IsNullOrEmpty( path ) ) return [];
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetAllFacePaps( x.Value.SkeletonId, path ) );
        }

        public static string ToTmbPath( string key ) => ( string.IsNullOrEmpty( key ) || key.Contains( "[SKL_ID]" ) ) ? string.Empty : $"chara/action/{key}.tmb";

        public static string ToVfxPath( string key ) => string.IsNullOrEmpty( key ) ? string.Empty : $"vfx/common/eff/{key}.avfx";
    }
}
