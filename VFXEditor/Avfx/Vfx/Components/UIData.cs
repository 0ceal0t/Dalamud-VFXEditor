using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public abstract class UIData : UIItem {
        public List<UIItem> Tabs = new();
        public UIItemSplitView<UIItem> SplitView;

        public UIData() {
            SplitView = new UIItemSplitView<UIItem>( Tabs );
        }

        public virtual void Dispose() { }

        public override string GetDefaultText() => "Data";

        public override void Draw( string parentId ) {
            DrawBody( parentId );
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Data";
            SplitView.Draw( id );
        }

        public override bool IsAssigned() => true;
    }
}
