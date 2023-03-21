namespace VfxEditor.Select.Sheets {
    public class XivCharacterEid {
        public readonly string Name;
        public readonly string Path;

        public XivCharacterEid(string name, string skeletonId ) {
            Name = name;
            Path = $"chara/human/{skeletonId}/skeleton/base/b0001/eid_{skeletonId}b0001.eid";
        }
    }
}
