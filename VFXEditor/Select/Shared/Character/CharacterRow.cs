namespace VfxEditor.Select.Shared.Character {
    public class CharacterRow {
        public readonly string Name;
        public readonly string SkeletonId;

        public CharacterRow( string name, string skeletonId ) {
            Name = name;
            SkeletonId = skeletonId;
        }

        public string EidPath => $"chara/human/{SkeletonId}/skeleton/base/b0001/eid_{SkeletonId}b0001.eid";

        public string GetLoopPap( int poseId ) => "chara/human/" + SkeletonId + "/animation/a0001/bt_common/emote/pose" + poseId.ToString().PadLeft( 2, '0' ) + "_loop.pap";

        public string GetStartPap( int poseId ) => "chara/human/" + SkeletonId + "/animation/a0001/bt_common/emote/pose" + poseId.ToString().PadLeft( 2, '0' ) + "_start.pap";

        public string GetPap( string file ) => "chara/human/" + SkeletonId + "/animation/a0001/bt_common/resident/" + file + ".pap";
    }
}
