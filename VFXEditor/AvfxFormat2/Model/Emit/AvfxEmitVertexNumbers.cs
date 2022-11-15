using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitVertexNumbers : AvfxBase {
        public readonly List<AvfxVertexNumber> VertexNumbers = new();

        public AvfxEmitVertexNumbers() : base( "VNum" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 2; i++ ) VertexNumbers.Add( new AvfxVertexNumber( reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var num in VertexNumbers ) num.Write( writer );
        }
    }

    public class AvfxVertexNumber {
        public readonly ParsedShort Number = new( "Order" );

        public AvfxVertexNumber() { }

        public AvfxVertexNumber( BinaryReader reader ) => Number.Read( reader );

        public void Write( BinaryWriter writer ) => Number.Write( writer );
    }
}
