using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitVertexes : AvfxBase {
        public readonly List<AvfxEmitVertex> EmitVertexes = [];

        public AvfxEmitVertexes() : base( "VEmt" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 28; i++ ) EmitVertexes.Add( new AvfxEmitVertex( reader ) );
        }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in EmitVertexes ) vert.Write( writer );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }
    }

    public class AvfxEmitVertex {
        public readonly ParsedFloat3 Position = new( "##Position" );
        public readonly ParsedFloat3 Normal = new( "##Normal" );
        public readonly ParsedIntColor Color = new( "##Color" );

        public AvfxEmitVertex() { }

        public AvfxEmitVertex( BinaryReader reader ) {
            Position.Read( reader );
            Normal.Read( reader );
            Color.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Position.Write( writer );
            Normal.Write( writer );
            Color.Write( writer );
        }
    }
}
