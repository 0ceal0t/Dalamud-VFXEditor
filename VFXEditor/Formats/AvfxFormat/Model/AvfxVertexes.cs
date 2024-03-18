using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace VfxEditor.AvfxFormat {
    public class AvfxVertexes : AvfxBase {
        public readonly List<AvfxVertex> Vertexes = [];

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
            Position = ReadHalf4( reader );

            for( var i = 0; i < 4; i++ ) Normal[i] = reader.ReadByte() - 128;
            for( var i = 0; i < 4; i++ ) Tangent[i] = reader.ReadByte() - 128;
            for( var i = 0; i < 4; i++ ) Color[i] = reader.ReadByte();

            Uv1 = ReadHalf2( reader );
            Uv2 = ReadHalf2( reader );
            Uv3 = ReadHalf2( reader );
            Uv4 = ReadHalf2( reader );
        }

        public void Write( BinaryWriter writer ) {
            WriteHalf4( Position, writer );

            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )( Normal[i] + 128 ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )( Tangent[i] + 128 ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )Color[i] );

            WriteHalf2( Uv1, writer );
            WriteHalf2( Uv2, writer );
            WriteHalf2( Uv3, writer );
            WriteHalf2( Uv4, writer );
        }

        private static Vector2 ReadHalf2( BinaryReader reader ) => new( ReadHalf( reader ), ReadHalf( reader ) );

        private static Vector4 ReadHalf4( BinaryReader reader ) => new( ReadHalf( reader ), ReadHalf( reader ), ReadHalf( reader ), ReadHalf( reader ) );

        private static float ReadHalf( BinaryReader reader ) => AvfxBase.BytesToHalf( reader.ReadBytes( 2 ) );

        private static void WriteHalf2( Vector2 data, BinaryWriter writer ) {
            WriteHalf( data.X, writer );
            WriteHalf( data.Y, writer );
        }

        private static void WriteHalf4( Vector4 data, BinaryWriter writer ) {
            WriteHalf( data.X, writer );
            WriteHalf( data.Y, writer );
            WriteHalf( data.Z, writer );
            WriteHalf( data.W, writer );
        }

        private static void WriteHalf( float val, BinaryWriter writer ) => writer.Write( AvfxBase.HalfToBytes( val ) );
    }
}
