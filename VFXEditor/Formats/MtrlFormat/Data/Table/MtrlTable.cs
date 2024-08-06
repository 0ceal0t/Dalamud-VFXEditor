using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MtrlFormat.Data.Color;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    public class MtrlTable : MtrlTableBase {
        public readonly List<MtrlColorRow> Rows = [];
        public readonly MtrlColorRowSplitView<MtrlColorRow> RowView;

        public MtrlTable( MtrlFile file ) : base( file ) {
            for( var i = 0; i < 32; i++ ) Rows.Add( new( this ) );
            RowView = new( Rows );
        }

        public MtrlTable( MtrlFile file, BinaryReader reader, long dataEnd ) : this( file ) {
            foreach( var row in Rows ) row.Read( reader );

            // Read dye rows
            if( !file.DyeTableEnabled || ( int )( dataEnd - reader.BaseStream.Position ) < ( int )DyeTableSize.Extended ) return;
            foreach( var row in Rows ) row.ReadDye( reader );
        }

        public override void Write( BinaryWriter writer ) {
            if( File.ColorTableEnabled ) foreach( var row in Rows ) row.Write( writer );
            if( File.DyeTableEnabled ) foreach( var row in Rows ) row.WriteDye( writer );
        }

        public override void Draw() {
            // TODO: draw dye
            RowView.Draw();
        }
    }
}
