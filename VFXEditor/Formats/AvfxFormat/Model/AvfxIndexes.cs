using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxIndexes : AvfxBase {
        public readonly List<AvfxIndex> Indexes = new();

        public AvfxIndexes() : base( "VIdx" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 6; i++ ) Indexes.Add( new AvfxIndex( reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var index in Indexes ) index.Write( writer );
        }
    }

    public class AvfxIndex {
        public int I1;
        public int I2;
        public int I3;

        public AvfxIndex( int i1, int i2, int i3 ) {
            I1 = i1;
            I2 = i2;
            I3 = i3;
        }

        public AvfxIndex( BinaryReader reader ) {
            I1 = reader.ReadInt16();
            I2 = reader.ReadInt16();
            I3 = reader.ReadInt16();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( short )I1 );
            writer.Write( ( short )I2 );
            writer.Write( ( short )I3 );
        }
    }
}
