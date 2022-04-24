using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public class UIParameterTabs : UIItem {
        public string Name;
        public List<UIItem> Items = new();

        public UIParameterTabs( string name ) {
            Name = name;
        }

        public void Add( UIItem item ) {
            Items.Add( item );
        }

        public override void DrawBody( string parentId ) {
            DrawListTabs( Items, parentId );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => true;
    }
}
