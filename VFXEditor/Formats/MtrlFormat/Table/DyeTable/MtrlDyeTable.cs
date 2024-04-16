using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Table.DyeTable {
    public class MtrlDyeTable {
        public const int Size = 16 * MtrlDyeTableRow.Size;

        public readonly List<MtrlDyeTableRow> Rows = [];

        public MtrlDyeTable() {
            for( var i = 0; i < 16; i++ ) Rows.Add( new() );
        }

        public MtrlDyeTable( BinaryReader reader, long size ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( reader ) );
        }

        public void Write( BinaryWriter writer ) => Rows.ForEach( x => x.Write( writer ) );
    }
}
