using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Event {
    public class EventRow : ISelectItem {
        public readonly string Name;
        public readonly string Path;
        public readonly int RowId;

        public EventRow( int rowId, string path, string name ) {
            RowId = rowId;
            Path = path;
            Name = name;
        }

        public string GetName() => Name;
    }
}