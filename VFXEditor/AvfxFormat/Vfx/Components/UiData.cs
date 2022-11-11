using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiData : UiItem {
        public readonly List<UiItem> Tabs = new();
        public readonly UiDisplaySplitView<UiItem> SplitView;

        public UiData() {
            SplitView = new UiDisplaySplitView<UiItem>( Tabs );
        }

        public override string GetDefaultText() => "Data";

        public override void DrawInline( string parentId ) {
            SplitView.DrawInline( $"{parentId}/Data" );
        }

        public virtual void Enable() { }

        public virtual void Disable() { }
    }
}
