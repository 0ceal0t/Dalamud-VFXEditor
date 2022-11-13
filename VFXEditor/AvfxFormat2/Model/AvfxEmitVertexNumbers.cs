using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.AVFXLib.Model;

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
        public int Num;

        public AvfxVertexNumber( int num ) {
            Num = num;
        }

        public AvfxVertexNumber( BinaryReader reader ) {
            Num = reader.ReadInt16();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( short )Num );
        }
    }
}
