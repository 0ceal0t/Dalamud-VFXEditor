using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class UiParsedFloat2 : UiParsedFloatVector<Vector2> {
        public UiParsedFloat2( string name, ParsedFloat p1, ParsedFloat p2 ) : base( name, p1, p2 ) { }

        protected override bool DrawInput( ref Vector2 value ) => ImGui.InputFloat2( Name, ref value, format: FloatFormat );
    }
}
