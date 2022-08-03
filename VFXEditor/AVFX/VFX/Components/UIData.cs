using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public abstract class UIData : UIItem {
        public readonly List<UIItem> Tabs = new();
        public readonly UIItemSplitView<UIItem> SplitView;

        public UIData() {
            SplitView = new UIItemSplitView<UIItem>( Tabs );
        }

        public override string GetDefaultText() => "Data";

        public override void DrawInline( string parentId ) {
            SplitView.DrawInline( $"{parentId}/Data" );
        }

        public virtual void Dispose() { }
    }
}
