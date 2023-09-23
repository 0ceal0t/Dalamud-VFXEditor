using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace VfxEditor.Select.Shared.Character {
    public class CharacterRow {
        public readonly string Name;
        public readonly string SkeletonId;
        public readonly int HairOffset;

        public string AtchPath => $"chara/xls/attachoffset/{SkeletonId}.atch";

        public CharacterRow( string name, RaceData race ) {
            Name = name;
            SkeletonId = race.SkeletonId;
            HairOffset = race.HairOffset;
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
            return ret;
        }

        public List<int> GetFaceIds() => SelectDataUtils.FaceMap.TryGetValue( SkeletonId, out var faces ) ? faces : new List<int>();
    }
}
