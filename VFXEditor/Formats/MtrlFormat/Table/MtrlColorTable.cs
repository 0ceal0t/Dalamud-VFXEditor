using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTable {
        public const int Size = 16 * MtrlColorTableRow.Size;

        public readonly List<MtrlColorTableRow> Rows = new();
        private readonly MtrlColorTableSplitView RowView;

        public MtrlColorTable( MtrlFile file ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( file ) );
            RowView = new( file, Rows );
        }

        public MtrlColorTable( MtrlFile file, BinaryReader reader ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( file, reader ) );
            RowView = new( file, Rows );
        }

        public void Draw() => RowView.Draw();

        public void Write( BinaryWriter writer ) => Rows.ForEach( x => x.Write( writer ) );
    }
}
