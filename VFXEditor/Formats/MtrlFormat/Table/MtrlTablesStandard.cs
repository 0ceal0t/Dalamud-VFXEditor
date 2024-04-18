using System.IO;
using VfxEditor.Formats.MtrlFormat.Table.Color;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlTablesStandard : MtrlTables {
        public MtrlTablesStandard( MtrlFile file ) : base( file ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new MtrlColorTableRowStandard( file ) );
            foreach( var row in Rows ) row.InitDye();
        }

        public MtrlTablesStandard( MtrlFile file, BinaryReader reader, long dataEnd ) : base( file ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new MtrlColorTableRowStandard( file, reader ) );

            if( file.DyeTableEnabled && ( int )( dataEnd - reader.BaseStream.Position ) >= ( int )DyeTableSize.Standard ) {
                foreach( var row in Rows ) row.InitDye( reader );
            }
            else {
                foreach( var row in Rows ) row.InitDye();
            }
        }
    }
}
