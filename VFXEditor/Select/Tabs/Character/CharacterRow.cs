namespace VfxEditor.Select.Tabs.Character {
    public class CharacterRow {
        public readonly string Name;
        public readonly RacialData Data;

        public string SkeletonId => Data.SkeletonId;
        public string AtchPath => $"chara/xls/attachoffset/{SkeletonId}.atch";

        public CharacterRow( string name, RacialData data ) {
            Name = name;
            Data = data;
        }

        public string GetLoopPap( int poseId ) => $"chara/human/{SkeletonId}/animation/a0001/bt_common/emote/pose" + poseId.ToString().PadLeft( 2, '0' ) + "_loop.pap";

        public string GetStartPap( int poseId ) => $"chara/human/{SkeletonId}/animation/a0001/bt_common/emote/pose" + poseId.ToString().PadLeft( 2, '0' ) + "_start.pap";

        public string GetPap( string path ) => $"chara/human/{SkeletonId}/animation/a0001/bt_common/resident/{path}.pap";

        public string GetBodyMaterial( int id, string suffix ) => $"chara/human/{SkeletonId}/obj/body/b{Pad( id )}/material/v0001/mt_{SkeletonId}b{Pad( id )}_{suffix}.mtrl";

        public string GetEarMaterial( int id, string suffix ) => $"chara/human/{SkeletonId}/obj/zear/z{Pad( id )}/material/mt_{SkeletonId}z{Pad( id )}_{suffix}.mtrl";

        public string GetTailMaterial( int id, string suffix ) => $"chara/human/{SkeletonId}/obj/tail/t{Pad( id )}/material/v0001/mt_{SkeletonId}t{Pad( id )}_{suffix}.mtrl";

        public string GetHairMaterial( int id, string suffix ) => $"chara/human/{SkeletonId}/obj/hair/h{Pad( id )}/material/v0001/mt_{SkeletonId}h{Pad( id )}_{suffix}.mtrl";

        public string GetFaceMaterial( int id, string suffix ) => $"chara/human/{SkeletonId}/obj/face/f{Pad( id )}/material/mt_{SkeletonId}f{Pad( id )}_{suffix}.mtrl";

        public RacialOptions GetOptions() => SelectDataUtils.RacialOptions.TryGetValue( SkeletonId, out var data ) ? data : new();

        private static string Pad( int id ) => id.ToString().PadLeft( 4, '0' );
    }
}