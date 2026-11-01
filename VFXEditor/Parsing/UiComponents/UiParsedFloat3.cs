using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class UiParsedFloat3 : UiParsedFloatVector<Vector3> {
        public UiParsedFloat3( string name, ParsedFloat p1, ParsedFloat p2, ParsedFloat p3 ) : base( name, p1, p2, p3 ) { }

        protected override bool DrawInput( ref Vector3 value ) => ImGui.InputFloat3( Name, ref value, format: FloatFormat );
    }
}
