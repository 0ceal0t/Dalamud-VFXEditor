using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedQuat : ParsedSimpleBase<Vector3, Vector3> {
        public Quaternion Quat {
            get => ToQuaternion( Value );
            set {
                Value = ToEuler( value );
            }
        }

        // Value is XYZ angles in radians
        public ParsedQuat( string name, Vector3 defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedQuat( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var w = reader.ReadSingle();
            Value = ToEuler( new( x, y, z, w ) );
        }

        public override void Write( BinaryWriter writer ) {
            var q = Quat;
            writer.Write( q.X );
            writer.Write( q.Y );
            writer.Write( q.Z );
            writer.Write( q.W );
        }

        public override void Draw( CommandManager manager ) {
            using var _ = ImRaii.PushId( Name );
            Copy( manager );

            if( UiUtils.DrawRadians3( Name, Value, out var newValue ) ) {
                manager.Add( new ParsedSimpleCommand<Vector3>( this, newValue ) );
            }
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
                v.Z = 0;
            }
            else if( test < -0.4995f ) {
                v.Y = -2f * ( float )Math.Atan2( q.X, q.W );
                v.X = -( float )Math.PI / 2;
                v.Z = 0;
            }
            else {
                double sqx = q.X * q.X;
                double sqy = q.Y * q.Y;
                double sqz = q.Z * q.Z;

                v.Y = ( float )Math.Atan2( ( 2 * q.Y * q.W ) - ( 2 * q.X * q.Z ), 1 - ( 2 * sqy ) - ( 2 * sqz ) );
                v.X = ( float )Math.Asin( 2 * test );
                v.Z = ( float )Math.Atan2( ( 2 * q.X * q.W ) - ( 2 * q.Y * q.Z ), 1 - ( 2 * sqx ) - ( 2 * sqz ) );
            }

            return NormalizeAngles( v );
        }

        public static Vector3 NormalizeAngles( Vector3 v ) {
            v.X = NormalizeAngle( v.X );
            v.Y = NormalizeAngle( v.Y );
            v.Z = NormalizeAngle( v.Z );
            return v;
        }

        public static float NormalizeAngle( float angle ) {
            while( angle > 180 )
                angle -= 360;

            while( angle < -180 )
                angle += 360;

            return angle;
        }

        protected override Dictionary<string, Vector3> GetCopyMap( CopyManager manager ) => manager.Vector3s;

        protected override Vector3 ToCopy() => Value;

        protected override Vector3 FromCopy( Vector3 val ) => val;
    }
}
