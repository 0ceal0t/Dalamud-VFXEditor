using Dalamud.Interface.Utility.Raii;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MtrlFormat.Table.Color {
    public class MtrlColorTableSplitView : UiSplitView<MtrlColorTableRow> {
        public readonly MtrlTables Tables;

        public MtrlColorTableSplitView( MtrlTables tables ) : base( "Row", tables.Rows, false ) {
            Tables = tables;
        }

        protected override void DrawLeftColumn() {
            var items = Items.GetRange( 0, Tables.Count );

            if( Selected != null && !items.Contains( Selected ) ) Selected = null;
            for( var idx = 0; idx < items.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                items[idx].DrawLeftItem( idx, ref Selected );
            }
        }
    }
}
