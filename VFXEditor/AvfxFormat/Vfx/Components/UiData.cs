using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiData : UiItem {
        public readonly List<UiItem> Tabs = new();
        public readonly UiItemSplitView<UiItem> SplitView;

        public UiData() {
            SplitView = new UiItemSplitView<UiItem>( Tabs );
        }

        public override string GetDefaultText() => "Data";

        public override void DrawInline( string parentId ) {
            SplitView.DrawInline( $"{parentId}/Data" );
        }

        public virtual void Dispose() { }
    }
}
