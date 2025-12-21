using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedFloat2 : ParsedSimpleBase<Vector2>
    {
        public bool HighPrecision = true;
        public ParsedFloat2( string name, Vector2 value ) : base( name, value ) { }

        public ParsedFloat2( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
        }

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat2( Name, ref value, format: HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f" ) ) Update( value );
        }
    }
}
