using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedQuatDouble : ParsedSimpleBase<(Double4, Vector3)> { // value is (Quat, Euler)
        public Vector3 Euler {
            get => Value.Item2;
            set {
                var quat = ParsedQuat.ToQuaternion( value );
                Value = (new( quat.X, quat.Y, quat.Z, quat.W ), value);
            }
        }

        public Double4 Quaternion {
            get => Value.Item1;
            set {
                var quat = new Quaternion(
                    ( float )value.X,
                    ( float )value.Y,
                    ( float )value.Z,
                    ( float )value.W );
                Value = (value, ParsedQuat.ToEuler( quat ));
            }
        }

        public Quaternion FloatQuaternion => new(
            ( float )Value.Item1.X,
            ( float )Value.Item1.Y,
            ( float )Value.Item1.Z,
            ( float )Value.Item1.W );

        public ParsedQuatDouble( string name ) : base( name ) {
            Value = (new( 0, 0, 0, 1 ), new( 0 ));
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Quaternion = new( reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble() );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.Item1.X );
            writer.Write( Value.Item1.Y );
            writer.Write( Value.Item1.Z );
            writer.Write( Value.Item1.W );
        }

        protected override void DrawBody() {
            var euler = Euler;
            if( UiUtils.DrawRadians3( Name, euler, out var newEuler ) ) {
                var quat = ParsedQuat.ToQuaternion( newEuler );
                Update( (new( quat.X, quat.Y, quat.Z, quat.W ), newEuler) );
            }
        }
    }
}
