using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxVertexes : AvfxBase {
        public readonly List<AvfxVertex> Vertexes = new();

        public AvfxVertexes() : base( "VDrw" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < size / 36; i++ ) Vertexes.Add( new AvfxVertex( reader ) );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in Vertexes ) vert.Write( writer );
        }
    }

    public class AvfxVertex {
        public float[] Position = new float[4];
        public int[] Normal = new int[4];
        public int[] Tangent = new int[4];
        public int[] Color = new int[4];
        public float[] UV1 = new float[4];
        public float[] UV2 = new float[4];

        public AvfxVertex() { }

        public AvfxVertex( BinaryReader reader ) {
            for( var i = 0; i < 4; i++ ) Position[i] = AvfxBase.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            for( var i = 0; i < 4; i++ ) Normal[i] = reader.ReadByte() - 128;
            for( var i = 0; i < 4; i++ ) Tangent[i] = reader.ReadByte() - 128;
            for( var i = 0; i < 4; i++ ) Color[i] = reader.ReadByte();
            for( var i = 0; i < 4; i++ ) UV1[i] = AvfxBase.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            for( var i = 0; i < 4; i++ ) UV2[i] = AvfxBase.Bytes2ToFloat( reader.ReadBytes( 2 ) );
        }

        public void Write( BinaryWriter writer ) {
            for( var i = 0; i < 4; i++ ) writer.Write( AvfxBase.FloatTo2Bytes( Position[i] ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )( Normal[i] + 128 ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )( Tangent[i] + 128 ) );
            for( var i = 0; i < 4; i++ ) writer.Write( ( byte )Color[i] );
            for( var i = 0; i < 4; i++ ) writer.Write( AvfxBase.FloatTo2Bytes( UV1[i] ) );
            for( var i = 0; i < 4; i++ ) writer.Write( AvfxBase.FloatTo2Bytes( UV2[i] ) );
        }
    }
}
