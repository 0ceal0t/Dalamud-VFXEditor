using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraOptionSplitView : UiSplitView<PenumbraOption> {
        public PenumbraOptionSplitView( List<PenumbraOption> items ) : base( "Option", items, true, false ) { }

        protected override void OnNew() {
            Items.Add( new() );
        }

        protected override void OnDelete( PenumbraOption item ) {
            item.Reset();
            Items.Remove( item );
        }

        protected override string GetText( PenumbraOption item, int idx ) => item.GetName();
    }
}
