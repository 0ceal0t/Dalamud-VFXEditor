using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public struct Double4 {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Double4( double x, double y, double z, double w ) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Double4( Vector4 vec4 ) {
            X = vec4.X;
            Y = vec4.Y;
            Z = vec4.Z;
            W = vec4.W;
        }

        public readonly Vector4 Vec4 => new( ( float )X, ( float )Y, ( float )Z, ( float )W );
    }

    public class ParsedDouble4 : ParsedDoubleVector<Double4> {
        public ParsedDouble4( string name, Double4 value ) : base( name, value ) { }

        public ParsedDouble4( string name ) : base( name ) { }

        protected override void DrawBody() {
            var value = Value.Vec4;
            if( ImGui.InputFloat4( Name, ref value, 0.0f, 0.0f, format: FloatFormat ) ) Update( new( value ) );
        }
    }
}
