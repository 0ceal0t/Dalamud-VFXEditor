using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitVertexNumbers : AvfxBase {
        public readonly List<AvfxVertexNumber> VertexNumbers = [];

        public AvfxEmitVertexNumbers() : base( "VNum" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 2; i++ ) VertexNumbers.Add( new AvfxVertexNumber( reader ) );
        }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var num in VertexNumbers ) num.Write( writer );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }
    }

    public class AvfxVertexNumber {
        public ParsedShort Order = new( "##Order" );

        public AvfxVertexNumber() { }

        public AvfxVertexNumber( BinaryReader reader ) => Order.Read( reader );

        public void Write( BinaryWriter writer ) => Order.Write( writer );
    }
}
