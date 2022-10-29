using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Model {
    public class AVFXIndexes : AVFXBase {
        public readonly List<AVFXIndex> Indexes = new();

        public AVFXIndexes() : base( "VIdx" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 6;
            for( var i = 0; i < count; i++ ) {
                Indexes.Add( new AVFXIndex( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var index in Indexes ) index.Write( writer );
        }

        public AVFXIndex Add() {
            SetAssigned( true );
            var index = new AVFXIndex( 0, 0, 0 );
            Indexes.Add( index );
            return index;
        }

        public void Add( AVFXIndex index ) {
            SetAssigned( true );
            Indexes.Add( index );
        }

        public void Remove( int idx ) {
            SetAssigned( true );
            Indexes.RemoveAt( idx );
        }

        public void Remove( AVFXIndex index ) {
            SetAssigned( true );
            Indexes.Remove( index );
        }
    }

    public class AVFXIndex {
        public int I1;
        public int I2;
        public int I3;

        public AVFXIndex( int i1, int i2, int i3 ) {
            I1 = i1;
            I2 = i2;
            I3 = i3;
        }

        public AVFXIndex( BinaryReader reader ) {
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
