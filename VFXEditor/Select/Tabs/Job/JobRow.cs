using VfxEditor.Select.Base;

namespace VfxEditor.Select.Tabs.Job {
    public class JobRow : ISelectItem {
        public readonly string Name;
        public readonly string Id;

        public JobRow( string name, string id ) {
            Name = name;
            Id = id;
        }

        public string GetName() => Name;
    }
}