using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.EventBase {
    public class EventBaseRow : ISelectItem {
        public readonly string Name;
        public readonly string Path;
        public readonly int RowId;

        public EventBaseRow( int rowId, string path, string name ) {
            RowId = rowId;
            Path = path;
            Name = name;
        }

        public string GetName() => Name;
    }
}