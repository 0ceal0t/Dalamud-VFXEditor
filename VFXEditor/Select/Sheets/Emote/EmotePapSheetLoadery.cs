using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VFXSelect.Select.Rows;

namespace VFXSelect.Select.Sheets {
    public class EmotePapSheetLoader : SheetLoader<XivEmotePap, XivEmotePapSelected> {
        public override void OnLoad() {
            var sheet = SheetManager.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Emote>().Where( x => !string.IsNullOrEmpty( x.Name ) );
            foreach( var item in sheet ) {
                Items.Add( new XivEmotePap( item ) );
            }
        }

        public override bool SelectItem( XivEmotePap item, out XivEmotePapSelected selectedItem ) {
            var allPaps = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            foreach( var papFile in item.PapFiles ) {
                var key = papFile.Key;
                var papDict = new Dictionary<string, Dictionary<string, string>>();

                if( papFile.Type == XivEmotePap.XivEmotePapType.Normal ) {
                    // bt_common, per race (chara/human/{RACE}/animation/a0001/bt_common/emote/add_yes.pap)
                    papDict.Add( "Action", SheetManager.FilterByExists( ActionPapSheetLoader.CalculateAllSkeletonPaths( $"bt_common/{key}.pap" ) ) ); // just a dummy node

                }
                else if( papFile.Type == XivEmotePap.XivEmotePapType.PerJob ) {
                    // chara/human/c0101/animation/a0001/bt_swd_sld/emote/battle01.pap
                    papDict = CalculateAllJobPaths( key );
                }
                else if( papFile.Type == XivEmotePap.XivEmotePapType.Facial ) {
                    // chara/human/c0101/animation/f0003/resident/face.pap
                    // chara/human/c0101/animation/f0003/resident/smile.pap
                    // chara/human/c0101/animation/f0003/nonresident/angry_cl.pap
                    papDict = CalculateAllFacePaths( key );
                }

                allPaps.Add( key, papDict );
            }

            selectedItem = new XivEmotePapSelected( item, allPaps );
            return true;
        }

        public static Dictionary<string, string> CalculateJobPath( string jobId, string path ) => SheetManager.FilterByExists( ActionPapSheetLoader.CalculateAllSkeletonPaths( $"{jobId}/{path}.pap" ) );

        public static Dictionary<string, Dictionary<string, string>> CalculateAllJobPaths( string path ) {
            Dictionary<string, Dictionary<string, string>> ret = new();
            if( string.IsNullOrEmpty( path ) ) return ret;

            ret.Add( "Warrior", CalculateJobPath( "bt_2ax_emp", path ) );
            ret.Add( "Paladin", CalculateJobPath( "bt_swd_sld", path ) );
            ret.Add( "Gunbreaker", CalculateJobPath( "bt_2gb_emp", path ) );
            ret.Add( "Dark Knight", CalculateJobPath( "bt_2sw_emp", path ) );
            ret.Add( "Astrologian", CalculateJobPath( "bt_2gl_emp", path ) );
            ret.Add( "Sage", CalculateJobPath( "bt_2ff_emp", path ) );
            ret.Add( "Scholar / Summoner", CalculateJobPath( "bt_2bk_emp", path ) );
            ret.Add( "White Mage / Black Mage", CalculateJobPath( "bt_stf_sld", path ) );
            ret.Add( "Machinist", CalculateJobPath( "bt_2gn_emp", path ) );
            ret.Add( "Dancer", CalculateJobPath( "bt_chk_chk", path ) );
            ret.Add( "Bard", CalculateJobPath( "bt_2bw_emp", path ) );
            ret.Add( "Samurai", CalculateJobPath( "bt_2kt_emp", path ) );
            ret.Add( "Dragoon", CalculateJobPath( "bt_2sp_emp", path ) );
            ret.Add( "Monk", CalculateJobPath( "bt_clw_clw", path ) );
            ret.Add( "Ninja", CalculateJobPath( "bt_dgr_dgr", path ) );
            ret.Add( "Reaper", CalculateJobPath( "bt_2km_emp", path ) );
            ret.Add( "Red Mage", CalculateJobPath( "bt_2rp_emp", path ) );
            ret.Add( "Blue Mage", CalculateJobPath( "bt_rod_emp", path ) );

            return ret;
        }

        public static Dictionary<string, string> CalculateFacePath( string modelId, string path, int minFace, int maxFace ) {
            Dictionary<string, string> ret = new();
            for ( var face = minFace; face <= maxFace; face++ ) {
                ret.Add( $"Face {face}", $"chara/human/{modelId}/animation/f{face:D4}/nonresident/{path}.pap" );
            }

            return SheetManager.FilterByExists( ret );
        }

        public static Dictionary<string, Dictionary<string, string>> CalculateAllFacePaths( string path ) {
            Dictionary<string, Dictionary<string, string>> ret = new();
            if( string.IsNullOrEmpty( path ) ) return ret;

            ret.Add( "Midlander M", CalculateFacePath( "c0101", path, 1, 7 ) );
            ret.Add( "Midlander F", CalculateFacePath( "c0201", path, 1, 5 ) );
            ret.Add( "Highlander M", CalculateFacePath( "c0301", path, 101, 104 ) );
            ret.Add( "Highlander F", CalculateFacePath( "c0401", path, 101, 104 ) );
            ret.Add( "Elezen M", CalculateFacePath( "c0501", path, 1, 4 ) );
            ret.Add( "Elezen F", CalculateFacePath( "c0601", path, 1, 4 ) );
            ret.Add( "Miquote M", CalculateFacePath( "c0701", path, 1, 4 ) );
            ret.Add( "Miquote F", CalculateFacePath( "c0801", path, 1, 4 ) );
            ret.Add( "Roegadyn M", CalculateFacePath( "c0901", path, 1, 4 ) );
            ret.Add( "Roegadyn F", CalculateFacePath( "c1001", path, 1, 4 ) );
            ret.Add( "Lalafell M", CalculateFacePath( "c1101", path, 1, 4 ) );
            ret.Add( "Lalafell F", CalculateFacePath( "c1201", path, 1, 4 ) );
            ret.Add( "AuRa M", CalculateFacePath( "c1301", path, 1, 4 ) );
            ret.Add( "AuRa F", CalculateFacePath( "c1401", path, 1, 4 ) );
            ret.Add( "Hrothgar M", CalculateFacePath( "c1501", path, 1, 8 ) );
            // 1601 coming soon (tm)
            ret.Add( "Viera M", CalculateFacePath( "c1701", path, 1, 4 ) );
            ret.Add( "Viera F", CalculateFacePath( "c1801", path, 1, 4 ) );

            return ret;
        }
    }
}
