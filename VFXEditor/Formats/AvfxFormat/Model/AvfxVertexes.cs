using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VfxEditor.AvfxFormat {
    public class AvfxVertexes : AvfxBase {
        public readonly List<AvfxVertex> Vertexes = new();

        public AvfxVertexes() : base( "VDrw" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 36; i++ ) Vertexes.Add( new AvfxVertex( reader ) );
        }

        public override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in Vertexes ) vert.Write( writer );
        }

        protected override IEnumerable<AvfxBase> GetChildren() {
            yield break;
        }
    }

    public class AvfxVertex {
        public Vector4 Position = new();

        public int[] Normal = new int[4];
        public int[] Tangent = new int[4];
        public int[] Color = new int[4];

        public Vector2 Uv1 = new();
        public Vector2 Uv2 = new();
        public Vector2 Uv3 = new();
        public Vector2 Uv4 = new();

        public AvfxVertex() { }

        public AvfxVertex( BinaryReader reader ) {
            Position = new(
                AvfxBase.Bytes2ToFloat( reader ),
                AvfxBase.Bytes2ToFloat( reader ),
                AvfxBase.Bytes2ToFloat( reader ),
                AvfxBase.Bytes2ToFloat( reader )
            );

            for( var i = 0; i < 4; i++ ) Normal[i] = reader.ReadByte() - 128;
            for( var i = 0; i < 4; i++ ) Tangent[i] = reader.ReadByte() - 128;
            for( var i = 0; i < 4; i++ ) Color[i] = reader.ReadByte();

            Uv1 = new( AvfxBase.Bytes2ToFloat( reader ), AvfxBase.Bytes2ToFloat( reader ) );
            Uv2 = new( AvfxBase.Bytes2ToFloat( reader ), AvfxBase.Bytes2ToFloat( reader ) );
            Uv3 = new( AvfxBase.Bytes2ToFloat( reader ), AvfxBase.Bytes2ToFloat( reader ) );
            Uv4 = new( AvfxBase.Bytes2ToFloat( reader ), AvfxBase.Bytes2ToFloat( reader ) );
        }

        public void Write( BinaryWriter writer ) {
            AvfxBase.FloatTo2Bytes( Position.X, writer );
            AvfxBase.FloatTo2Bytes( Position.Y, writer );
            AvfxBase.FloatTo2Bytes( Position.Z, writer );
            AvfxBase.FloatTo2Bytes( Position.W, writer );

            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )( Normal[i] + 128 ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )( Tangent[i] + 128 ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )Color[i] );

            AvfxBase.FloatTo2Bytes( Uv1.X, writer );
            AvfxBase.FloatTo2Bytes( Uv1.Y, writer );

            AvfxBase.FloatTo2Bytes( Uv2.X, writer );
            AvfxBase.FloatTo2Bytes( Uv2.Y, writer );

            AvfxBase.FloatTo2Bytes( Uv3.X, writer );
            AvfxBase.FloatTo2Bytes( Uv3.Y, writer );

            AvfxBase.FloatTo2Bytes( Uv4.X, writer );
            AvfxBase.FloatTo2Bytes( Uv4.Y, writer );
        }
    }
}
