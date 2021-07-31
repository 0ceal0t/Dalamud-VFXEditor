using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;

namespace AVFXLib.Models {
    public class Vertex {
        public static readonly int SIZE = 36;

        public float[] Position = new float[4]; // 2 bytes (half) =  8
        public int[] Normal = new int[4]; // 1 byte = 4, starts at 8  UNSIGNED?   last one = 003C
        public int[] Tangent = new int[4]; // 1 byte = 4, starts at 12 UNSIGNED?  last one = 7F
        public int[] Color = new int[4]; // 1 byte = 4, starts at 16 UNSIGNED?    last one = 7F
        public float[] UV1 = new float[4]; // 2 bytes = 8, starts at 20 // split into 2 x,y
        public float[] UV2 = new float[4]; // 2 bytes = 8, starts at 28
        // total = 36

        public Vertex() { }

        public Vertex( byte[] bytes ) {
            var ms = new MemoryStream( bytes );
            var reader = new BinaryReader( ms );
            for( var i = 0; i < 4; i++ ) {
                Position[i] = Util.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                Normal[i] = Util.Bytes1ToInt( reader.ReadBytes( 1 ) ) - 128;
            }
            for( var i = 0; i < 4; i++ ) {
                Tangent[i] = Util.Bytes1ToInt( reader.ReadBytes( 1 ) ) - 128;
            }
            for( var i = 0; i < 4; i++ ) {
                Color[i] = Util.Bytes1ToInt( reader.ReadBytes( 1 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                UV1[i] = Util.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                UV2[i] = Util.Bytes2ToFloat( reader.ReadBytes( 2 ) );
            }
            ms.Close();
        }

        public byte[] ToBytes() {
            var ms = new MemoryStream();
            var writer = new BinaryWriter( ms );
            for( var i = 0; i < 4; i++ ) {
                writer.Write( Util.FloatTo2Bytes( Position[i] ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( Util.IntTo1Bytes( Normal[i] + 128 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( Util.IntTo1Bytes( Tangent[i] + 128 ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( Util.IntTo1Bytes( Color[i] ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( Util.FloatTo2Bytes( UV1[i] ) );
            }
            for( var i = 0; i < 4; i++ ) {
                writer.Write( Util.FloatTo2Bytes( UV2[i] ) );
            }
            return ms.ToArray();
        }
    }
}
