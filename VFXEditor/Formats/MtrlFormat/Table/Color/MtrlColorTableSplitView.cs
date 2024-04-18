using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MtrlFormat.Table.Color {
    public class MtrlColorTableSplitView : UiSplitView<MtrlColorTableRow> {
        public MtrlColorTableSplitView( List<MtrlColorTableRow> items ) : base( "Row", items, false ) { }

        protected override bool DrawLeftItem( MtrlColorTableRow item, int idx ) {
            using var _ = ImRaii.PushId( idx );
            item.DrawLeftItem( idx, ref Selected );
            return false;
        }
    }
}
