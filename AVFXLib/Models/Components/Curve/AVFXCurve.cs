using AVFXLib.AVFX;
using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXCurve : Base {
        public LiteralEnum<CurveBehavior> PreBehavior = new( "BvPr" );
        public LiteralEnum<CurveBehavior> PostBehavior = new( "BvPo" );
        public LiteralEnum<RandomType> Random = new( "RanT" );

        public List<AVFXKey> Keys = new();
        private readonly List<Base> Attributes;

        public AVFXCurve( string avfxName ) : base( avfxName ) {
            Attributes = new List<Base>( new Base[]{
                PreBehavior,
                PostBehavior,
                Random
            } );
        }

        public override void Read( AVFXNode node ) {
            Assigned = true;
            ReadAVFX( Attributes, node );
            // READ KEYS
            // each key is 16 bytes -> 2:time 2:type 4:x 4:y 4:z
            foreach( var item in node.Children ) {
                if( item.Name == "Keys" ) // found the key, parse it
                {
                    var leaf = ( AVFXLeaf )item;
                    var contents = leaf.Contents;
                    for( var idx = 0; idx < ( int )contents.Length / 16; idx++ ) {
                        var oneKey = new byte[16];
                        Buffer.BlockCopy( contents, idx * 16, oneKey, 0, 16 );
                        Keys.Add( new AVFXKey( oneKey ) );
                    }
                    break;
                }
            }
        }

        public override void ToDefault() {
            Assigned = true;
            SetDefault( Attributes );
            Keys = new List<AVFXKey>();
        }

        public AVFXKey AddKey() {
            var key = new AVFXKey( KeyType.Linear, 0, 1, 1, 1 );
            Keys.Add( key );
            return key;
        }
        public void AddKey( AVFXKey item ) {
            Keys.Add( item );
        }
        public void RemoveKey( int idx ) {
            Keys.RemoveAt( idx );
        }
        public void RemoveKey( AVFXKey item ) {
            Keys.Remove( item );
        }

        public override AVFXNode ToAVFX() {
            var curveAvfx = new AVFXNode( AVFXName );

            // add keyCount
            curveAvfx.Children.Add( new AVFXLeaf( "KeyC", 4, BitConverter.GetBytes( Keys.Count ) ) );
            PutAVFX( curveAvfx, Attributes );

            if( Keys.Count == 0 ) { return curveAvfx; }

            // concat the keys
            var keyBArray = new byte[16 * Keys.Count];
            var offset = 0;
            foreach( var key in Keys ) {
                Buffer.BlockCopy( key.GetBytes(), 0, keyBArray, offset, 16 );
                offset += 16;
            }
            curveAvfx.Children.Add( new AVFXLeaf( "Keys", 16 * Keys.Count, keyBArray ) );
            return curveAvfx;
        }
    }

    public class AVFXKey {
        public KeyType Type;
        public int Time;

        public float X;
        public float Y;
        public float Z;

        public AVFXKey( KeyType type, int time, float x, float y, float z ) {
            Type = type;
            Time = time;
            X = x;
            Y = y;
            Z = z;
        }

        public AVFXKey( byte[] rawBytes ) {
            var xBytes = new byte[4];
            var yBytes = new byte[4];
            var zBytes = new byte[4];
            var timeBytes = new byte[2];
            var typeBytes = new byte[2];

            Buffer.BlockCopy( rawBytes, 0, timeBytes, 0, 2 );
            Buffer.BlockCopy( rawBytes, 2, typeBytes, 0, 2 );
            Buffer.BlockCopy( rawBytes, 4, xBytes, 0, 4 );
            Buffer.BlockCopy( rawBytes, 8, yBytes, 0, 4 );
            Buffer.BlockCopy( rawBytes, 12, zBytes, 0, 4 );

            Time = Util.Bytes2ToInt( timeBytes );
            Type = ( KeyType )Util.Bytes2ToInt( typeBytes );
            X = Util.Bytes4ToFloat( xBytes );
            Y = Util.Bytes4ToFloat( yBytes );
            Z = Util.Bytes4ToFloat( zBytes );
        }

        public byte[] GetBytes() {
            // time[1] 00 type[1] 00 x[4] y[4] z[4]
            var bytes = new byte[16];
            var timeBytes = Util.IntTo2Bytes( Time );
            var typeBytes = Util.IntTo2Bytes( ( int )Type );
            var xBytes = Util.FloatTo4Bytes( X );
            var yBytes = Util.FloatTo4Bytes( Y );
            var zBytes = Util.FloatTo4Bytes( Z );
            Buffer.BlockCopy( timeBytes, 0, bytes, 0, 2 );
            Buffer.BlockCopy( typeBytes, 0, bytes, 2, 2 );
            Buffer.BlockCopy( xBytes, 0, bytes, 4, 4 );
            Buffer.BlockCopy( yBytes, 0, bytes, 8, 4 );
            Buffer.BlockCopy( zBytes, 0, bytes, 12, 4 );
            return bytes;
        }
    }
}
