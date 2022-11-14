using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitVertexes : AvfxBase {
        public readonly List<AvfxEmitVertex> EmitVertexes = new();

        public AvfxEmitVertexes() : base( "VEmt" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 28; i++ ) EmitVertexes.Add( new AvfxEmitVertex( reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in EmitVertexes ) vert.Write( writer );
        }
    }

    public class AvfxEmitVertex {
        public readonly ParsedFloat3 Position = new( "Position" );
        public readonly ParsedFloat3 Normal = new( "Normal" );
        public Vector4 Color = new( 0 );

        public AvfxEmitVertex() { }

        public AvfxEmitVertex( BinaryReader reader ) {
            Position.Read( reader, 0 );
            Normal.Read( reader, 0 );

            Color.X = reader.ReadByte() / 255f;
            Color.Y = reader.ReadByte() / 255f;
            Color.Z = reader.ReadByte() / 255f;
            Color.W = reader.ReadByte() / 255f;
        }

        public void Write( BinaryWriter writer ) {
            Position.Write( writer );
            Normal.Write( writer );

            writer.Write( ( byte )( Color.X * 255f) );
            writer.Write( ( byte )( Color.Y * 255f ) );
            writer.Write( ( byte )( Color.Z * 255f ) );
            writer.Write( ( byte )( Color.W * 255f ) );
        }
    }
}
