using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public class MtrlColorRowSplitView<T> : UiSplitView<T> where T : MtrlColorRowBase {
        public MtrlColorRowSplitView( List<T> rows ) : base( "Row", rows, false ) { }

        protected override void DrawLeftColumn() {
            if( Selected != null && !Items.Contains( Selected ) ) Selected = null;
            for( var idx = 0; idx < Items.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                if( Items[idx].DrawLeftItem( idx, Selected == Items[idx] ) ) Selected = Items[idx];
            }
        }
    }
}
