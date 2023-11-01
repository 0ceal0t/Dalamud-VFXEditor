using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Character {
    public class CharacterRow {
        public readonly string Name;
        public readonly RaceData Data;

        public string SkeletonId => Data.SkeletonId;
        public int HairOffset => Data.HairOffset;
        public string AtchPath => $"chara/xls/attachoffset/{SkeletonId}.atch";

        public CharacterRow( string name, RaceData data ) {
            Name = name;
            Data = data;
        }

        public string GetLoopPap( int poseId ) => $"chara/human/{SkeletonId}/animation/a0001/bt_common/emote/pose" + poseId.ToString().PadLeft( 2, '0' ) + "_loop.pap";

        public string GetStartPap( int poseId ) => $"chara/human/{SkeletonId}/animation/a0001/bt_common/emote/pose" + poseId.ToString().PadLeft( 2, '0' ) + "_start.pap";

        public string GetPap( string path ) => $"chara/human/{SkeletonId}/animation/a0001/bt_common/resident/{path}.pap";

        public List<int> GetHairIds() {
            var ret = new List<int>();
            var sheet = Dalamud.DataManager.GetExcelSheet<CharaMakeCustomize>();
            for( var hair = HairOffset; hair < HairOffset + SelectDataUtils.HairEntries; hair++ ) {
                var hairRow = sheet.GetRow( ( uint )hair );
                var hairId = ( int )hairRow.FeatureID;
                if( hairId == 0 ) continue;

                ret.Add( hairId );
            }
            ret.Sort();
            return ret;
        }

        public List<int> GetFaceIds() {
            var ret = SelectDataUtils.FaceMap.TryGetValue( SkeletonId, out var faces ) ? faces : new List<int>();
            ret.Sort();
            return ret;
        }
    }
}