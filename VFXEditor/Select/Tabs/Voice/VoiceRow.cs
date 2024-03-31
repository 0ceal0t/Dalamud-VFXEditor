using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Voice {
    public class VoiceRow : ISelectItem {
        public readonly string Name;
        public readonly string Id;
        public readonly int RowId;

        public VoiceRow( int rowId, string path, string name ) {
            RowId = rowId;
            Id = path;
            Name = name;
        }

        public string GetName() => Name;
    }
}