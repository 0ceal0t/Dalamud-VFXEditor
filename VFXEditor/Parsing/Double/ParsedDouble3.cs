using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public struct Double3 {
        public double X;
        public double Y;
        public double Z;

        public Double3( Vector3 vec3 ) {
            X = vec3.X;
            Y = vec3.Y;
            Z = vec3.Z;
        }

        public readonly Vector3 Vec3 => new( ( float )X, ( float )Y, ( float )Z );
    }

    public class ParsedDouble3 : ParsedSimpleBase<Double3> {
        public ParsedDouble3( string name, Double3 value ) : base( name, value ) { }

        public ParsedDouble3( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadDouble();
            Value.Y = reader.ReadDouble();
            Value.Z = reader.ReadDouble();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
        }

        protected override void DrawBody() {
            var value = Value.Vec3;
            if( ImGui.InputFloat3( Name, ref value ) ) Update( new( value ) );
        }
    }
}
