using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AVFXLib.Model {
    public class AVFXVertexes : AVFXBase {
        public readonly List<AVFXVertex> Vertexes = new();

        public AVFXVertexes() : base( "VDrw" ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            var count = size / 36;
            for( var i = 0; i < count; i++ ) {
                Vertexes.Add( new AVFXVertex( reader ) );
            }
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            foreach( var vert in Vertexes ) vert.Write( writer );
        }

        public AVFXVertex Add() {
            SetAssigned( true );
            var vert = new AVFXVertex();
            Vertexes.Add( vert );
            return vert;
        }

        public void Add( AVFXVertex vert ) {
            SetAssigned( true );
            Vertexes.Add( vert );
        }

        public void Remove( int idx ) {
            SetAssigned( true );
            Vertexes.RemoveAt( idx );
        }

        public void Remove( AVFXVertex vert ) {
            SetAssigned( true );
            Vertexes.Remove( vert );
        }
    }

    public class AVFXVertex {
        public float[] Position = new float[4];
        public int[] Normal = new int[4];
        public int[] Tangent = new int[4];
        public int[] Color = new int[4];
        public float[] UV1 = new float[4];
        public float[] UV2 = new float[4];

        public AVFXVertex() {
        }

        public AVFXVertex( BinaryReader reader ) {
            for( var i = 0; i < 4; i++ ) {
                Position[i] = AVFXBase.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                Normal[i] = reader.ReadByte() - 128;
            }
            for( var i = 0; i < 4; i++ ) {
                Tangent[i] = reader.ReadByte() - 128;
            }
            for( var i = 0; i < 4; i++ ) {
                Color[i] = reader.ReadByte();
            }
            for( var i = 0; i < 4; i++ ) {
                UV1[i] = AVFXBase.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                UV2[i] = AVFXBase.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            for( var i = 0; i < 4; i++ ) {
                writer.Write( AVFXBase.FloatTo2Bytes( Position[i] ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( ( byte )( Normal[i] + 128 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( ( byte )( Tangent[i] + 128 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( ( byte )Color[i] );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( AVFXBase.FloatTo2Bytes( UV1[i] ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( AVFXBase.FloatTo2Bytes( UV2[i] ) );
            }
        }
    }
}
