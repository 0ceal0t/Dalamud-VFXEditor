using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParameterTabs : UiItem {
        public string Name;
        public List<UiItem> Items = new();

        public UiParameterTabs( string name ) {
            Name = name;
        }

        public void Add( UiItem item ) {
            Items.Add( item );
        }

        public override void DrawInline( string parentId ) {
            DrawListTabs( Items, parentId );
        }

        public override string GetDefaultText() => Name;
    }
}
