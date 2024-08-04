using System;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedQuat : ParsedSimpleBase<(Quaternion, Vector3)> { // value is (Quat, Euler)
        public Vector3 Euler {
            get => Value.Item2;
            set {
                Value = (ToQuaternion( value ), value);
            }
        }

        public Quaternion Quaternion {
            get => Value.Item1;
            set {
                Value = (value, ToEuler( value ));
            }
        }

        public ParsedQuat( string name ) : base( name ) {
            Value = (new( 0, 0, 0, 1 ), new( 0 ));
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Quaternion = new( reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Quaternion.X );
            writer.Write( Quaternion.Y );
            writer.Write( Quaternion.Z );
            writer.Write( Quaternion.W );
        }

        protected override void DrawBody() {
            var euler = Euler;
            if( UiUtils.DrawRadians3( Name, euler, out var newEuler ) ) Update( (ToQuaternion( newEuler ), newEuler) );
        }

        // Yoinked from Ktisis, but in radians instead of degrees
        public static Quaternion ToQuaternion( Vector3 euler ) {
            double yaw = euler.Y;
            double pitch = euler.X;
            double roll = euler.Z;

            var c1 = Math.Cos( yaw / 2 );
            var s1 = Math.Sin( yaw / 2 );
            var c2 = Math.Cos( pitch / 2 );
            var s2 = Math.Sin( pitch / 2 );
            var c3 = Math.Cos( roll / 2 );
            var s3 = Math.Sin( roll / 2 );

            var c1c2 = c1 * c2;
            var s1s2 = s1 * s2;

            var x = ( c1c2 * s3 ) + ( s1s2 * c3 );
            var y = ( s1 * c2 * c3 ) + ( c1 * s2 * s3 );
            var z = ( c1 * s2 * c3 ) - ( s1 * c2 * s3 );
            var w = ( c1c2 * c3 ) - ( s1s2 * s3 );

            return new Quaternion( ( float )x, ( float )y, ( float )z, ( float )w );
        }

        public static Vector3 ToEuler( Quaternion q ) {
            var v = new Vector3();

            double test = ( q.X * q.Y ) + ( q.Z * q.W );

            if( test > 0.4995f ) {
                v.Y = 2f * ( float )Math.Atan2( q.X, q.Y );
                v.X = ( float )Math.PI / 2;
            }
            else if( test < -0.4995f ) {
                v.Y = -2f * ( float )Math.Atan2( q.X, q.W );
                v.X = -( float )Math.PI / 2;
            }
            else {
                double sqx = q.X * q.X;
                double sqy = q.Y * q.Y;
                double sqz = q.Z * q.Z;

                v.Y = ( float )Math.Atan2( ( 2 * q.Y * q.W ) - ( 2 * q.X * q.Z ), 1 - ( 2 * sqy ) - ( 2 * sqz ) );
                v.X = ( float )Math.Asin( 2 * test );
                v.Z = ( float )Math.Atan2( ( 2 * q.X * q.W ) - ( 2 * q.Y * q.Z ), 1 - ( 2 * sqx ) - ( 2 * sqz ) );
            }
            return v;
        }
    }
}
