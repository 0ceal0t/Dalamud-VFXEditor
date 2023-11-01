using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTable {
        public const int Size = 16 * MtrlColorTableRow.Size;

        public readonly List<MtrlColorTableRow> Rows = new();
        private readonly UiSplitView<MtrlColorTableRow> RowView;

        public MtrlColorTable() {
            for( var i = 0; i < 16; i++ ) Rows.Add( new() );
            RowView = new( "Row", Rows, false, false );
        }

        public MtrlColorTable( BinaryReader reader ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( reader ) );
            RowView = new( "Row", Rows, false, false );
        }

        public void Draw() => RowView.Draw();
    }
}
