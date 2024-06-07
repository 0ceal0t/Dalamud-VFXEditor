using System;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedQuat : ParsedSimpleBase<(Double4, Vector3)> { // value is (Quat, Euler)
        private readonly int Size;
        public Double4 Quaternion => Value.Item1;
        public Vector3 Eueler => Value.Item2;

        public ParsedQuat( string name, int size = 4 ) : base( name ) {
            Size = size;
            Value = (new( 0, 0, 0, 1 ), new( 0 ));
        }

        private double ReadElement( BinaryReader reader ) => Size switch {
            8 => reader.ReadDouble(),
            _ => reader.ReadSingle()
        };

        private void WriteElement( BinaryWriter writer, double data ) {
            if( Size == 8 ) writer.Write( ( double )data );
            else writer.Write( ( float )data );
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            var x = ReadElement( reader );
            var y = ReadElement( reader );
            var z = ReadElement( reader );
            var w = ReadElement( reader );
            Value = (new( x, y, z, w ), ToEuler( x, y, z, w ).Vec3);
        }

        public override void Write( BinaryWriter writer ) {
            WriteElement( writer, Quaternion.X );
            WriteElement( writer, Quaternion.Y );
            WriteElement( writer, Quaternion.Z );
            WriteElement( writer, Quaternion.W );
        }

        public void SetQuaternion( double x, double y, double z, double w ) {
            Value = (new( x, y, z, w ), ToEuler( x, y, z, w ).Vec3);
        }

        public void SetEueler( Vector3 euler ) {
            Value = (ToQuaternion( euler ), euler);
        }

        protected override void DrawBody() {
            var euler = Eueler;
            if( UiUtils.DrawRadians3( Name, euler, out var newEuler ) ) Update( (ToQuaternion( newEuler ), newEuler) );
        }

        // Yoinked from Ktisis, but in radians instead of degrees
        public static Double4 ToQuaternion( Vector3 euler ) {
            var yaw = euler.Y;
            var pitch = euler.X;
            var roll = euler.Z;

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

            return new Double4( x, y, z, w );
        }

        public static Double3 ToEuler( double x, double y, double z, double w ) {
            var v = new Double3();

            var test = ( x * y ) + ( z * w );

            if( test > 0.4995d ) {
                v.Y = 2d * Math.Atan2( x, y );
                v.X = Math.PI / 2;
                v.Z = 0;
            }
            else if( test < -0.4995d ) {
                v.Y = -2d * Math.Atan2( x, w );
                v.X = -Math.PI / 2;
                v.Z = 0;
            }
            else {
                var sqx = x * x;
                var sqy = y * y;
                var sqz = z * z;

                v.Y = Math.Atan2( ( 2 * y * w ) - ( 2 * x * z ), 1 - ( 2 * sqy ) - ( 2 * sqz ) );
                v.X = Math.Asin( 2 * test );
                v.Z = Math.Atan2( ( 2 * x * w ) - ( 2 * y * z ), 1 - ( 2 * sqx ) - ( 2 * sqz ) );
            }

            return NormalizeAngles( v );
        }

        public static Double3 NormalizeAngles( Double3 v ) {
            v.X = NormalizeAngle( v.X );
            v.Y = NormalizeAngle( v.Y );
            v.Z = NormalizeAngle( v.Z );
            return v;
        }

        public static double NormalizeAngle( double angle ) {
            while( angle > Math.PI / 2 )
                angle -= Math.PI;

            while( angle < -Math.PI / 2 )
                angle += Math.PI;

            return angle;
        }
    }
}
