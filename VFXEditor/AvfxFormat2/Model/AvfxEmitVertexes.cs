using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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
        public Vector3 Position = new( 0 );
        public Vector3 Normal = new( 0 );
        public Vector4 Color = new( 0 );

        public AvfxEmitVertex() { }

        public AvfxEmitVertex( BinaryReader reader ) {
            Position.X = reader.ReadSingle();
            Position.Y = reader.ReadSingle();
            Position.Z = reader.ReadSingle();

            Normal.X = reader.ReadSingle();
            Normal.Y = reader.ReadSingle();
            Normal.Z = reader.ReadSingle();

            Color.X = reader.ReadByte() / 255f;
            Color.Y = reader.ReadByte() / 255f;
            Color.Z = reader.ReadByte() / 255f;
            Color.W = reader.ReadByte() / 255f;
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Position.X );
            writer.Write( Position.Y );
            writer.Write( Position.Z );

            writer.Write( Normal.X );
            writer.Write( Normal.Y );
            writer.Write( Normal.Z );

            writer.Write( ( byte )( Color.X * 255f) );
            writer.Write( ( byte )( Color.Y * 255f ) );
            writer.Write( ( byte )( Color.Z * 255f ) );
            writer.Write( ( byte )( Color.W * 255f ) );
        }
    }
}
