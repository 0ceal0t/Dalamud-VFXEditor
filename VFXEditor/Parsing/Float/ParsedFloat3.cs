using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : ParsedFloatVector<Vector3> {
        public ParsedFloat3( string name, Vector3 value ) : base( name, value ) { }

        public ParsedFloat3( string name ) : base( name ) { }

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat3( Name, ref value, format: FloatFormat ) ) Update( value );
        }
    }
}
