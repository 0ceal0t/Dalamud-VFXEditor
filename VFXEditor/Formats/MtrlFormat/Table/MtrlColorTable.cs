using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTable {
        public const int Size = 16 * MtrlColorTableRow.Size;

        public readonly List<MtrlColorTableRow> Rows = new();
        private readonly MtrlColorTableSplitView RowView;

        public MtrlColorTable() {
            for( var i = 0; i < 16; i++ ) Rows.Add( new() );
            RowView = new( Rows );
        }

        public MtrlColorTable( BinaryReader reader ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( reader ) );
            RowView = new( Rows );
        }

        public void Draw() => RowView.Draw();

        public void Write( BinaryWriter writer ) => Rows.ForEach( x => x.Write( writer ) );
    }
}
