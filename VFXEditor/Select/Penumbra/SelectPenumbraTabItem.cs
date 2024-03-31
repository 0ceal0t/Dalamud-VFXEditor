using VfxEditor.Select.Base;

namespace VfxEditor.Select.Penumbra {
    public class SelectPenumbraTabItem : ISelectItem {
        public readonly string Name;

        public SelectPenumbraTabItem( string name ) {
            Name = name;
        }

        public string GetName() => Name;
    }
}
