namespace VfxEditor.Select2.Eid.Character {
    public class CharacterRow {
        public readonly string Name;
        public readonly string Path;

        public CharacterRow( string name, string skeletonId ) {
            Name = name;
            Path = $"chara/human/{skeletonId}/skeleton/base/b0001/eid_{skeletonId}b0001.eid";
        }
    }
}
