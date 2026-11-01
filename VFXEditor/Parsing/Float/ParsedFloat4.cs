using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat4 : ParsedFloatVector<Vector4> {
        public ParsedFloat4( string name, Vector4 value ) : base( name, value ) { }

        public ParsedFloat4( string name ) : base( name ) { }

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat4( Name, ref value, format: FloatFormat ) ) Update( value );
        }
    }
}
