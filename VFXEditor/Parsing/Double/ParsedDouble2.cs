using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public struct Double2 {
        public double X;
        public double Y;

        public Double2( Vector2 vec2 ) {
            X = vec2.X;
            Y = vec2.Y;
        }

        public readonly Vector2 Vec2 => new( ( float )X, ( float )Y );
    }

    public class ParsedDouble2 : ParsedSimpleBase<Double2> {
        public ParsedDouble2( string name, Double2 value ) : base( name, value ) { }

        public ParsedDouble2( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadDouble();
            Value.Y = reader.ReadDouble();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
        }

        protected override void DrawBody() {
            var value = Value.Vec2;
            if( ImGui.InputFloat2( Name, ref value ) ) Update( new( value ) );
        }
    }
}