using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Ui.Export.Penumbra {
    public class PenumbraOptionSplitView : UiSplitView<PenumbraOptionView> {
        public PenumbraOptionSplitView( List<PenumbraOptionView> items ) : base( "Option", items, true, false ) { }

        protected override void OnNew() {
            Items.Add( new() );
        }

        protected override void OnDelete( PenumbraOptionView item ) {
            item.Reset();
            Items.Remove( item );
        }

        protected override string GetText( PenumbraOptionView item, int idx ) => item.GetName();
    }
}
