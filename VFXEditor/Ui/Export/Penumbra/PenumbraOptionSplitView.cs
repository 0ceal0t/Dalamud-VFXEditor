using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraOptionSplitView : UiSplitView<PenumbraOption> {
        public PenumbraOptionSplitView( List<PenumbraOption> items ) : base( "Option", items, false ) { }

        protected override string GetText( PenumbraOption item, int idx ) => item.GetName();

        protected override void DrawControls() => DrawNewDeleteControls( OnNew, OnDelete );

        private void OnNew() {
            Items.Add( new() );
        }

        private void OnDelete( PenumbraOption item ) {
            item.Reset();
            Items.Remove( item );
        }
    }
}
