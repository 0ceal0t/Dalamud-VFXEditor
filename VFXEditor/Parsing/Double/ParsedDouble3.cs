using Dalamud.Bindings.ImGui;
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

    public class ParsedDouble3 : ParsedDoubleVector<Double3> {
        public ParsedDouble3( string name, Double3 value ) : base( name, value ) { }

        public ParsedDouble3( string name ) : base( name ) { }

        protected override void DrawBody() {
            var value = Value.Vec3;
            if( ImGui.InputFloat3( Name, ref value, 0.0f, 0.0f, format: FloatFormat ) ) Update( new( value ) );
        }
    }
}
