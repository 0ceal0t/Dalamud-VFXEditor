using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat2 : ParsedFloatVector<Vector2> {
        public ParsedFloat2( string name, Vector2 value ) : base( name, value ) { }

        public ParsedFloat2( string name ) : base( name ) { }

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat2( Name, ref value, format: FloatFormat ) ) Update( value );
        }
    }
}
