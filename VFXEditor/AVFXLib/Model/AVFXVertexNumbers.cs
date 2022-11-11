using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Model {
    public class AVFXVertexNumbers : AVFXBase {
        public readonly List<AVFXVertexNumber> VertexNumbers = new();

        public AVFXVertexNumbers() : base( "VNum" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 2;
            for( var i = 0; i < count; i++ ) {
                VertexNumbers.Add( new AVFXVertexNumber( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var num in VertexNumbers ) num.Write( writer );
        }
    }

    public class AVFXVertexNumber {
        public int Num;

        public AVFXVertexNumber( int num ) {
            Num = num;
        }

        public AVFXVertexNumber( BinaryReader reader ) {
            Num = reader.ReadInt16();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( short )Num );
        }
    }
}
