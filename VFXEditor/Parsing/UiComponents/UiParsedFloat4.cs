using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class UiParsedFloat4 : UiParsedFloatVector<Vector4> {
        public UiParsedFloat4( string name, ParsedFloat p1, ParsedFloat p2, ParsedFloat p3, ParsedFloat p4 ) : base( name, p1, p2, p3, p4 ) { }

        protected override bool DrawInput( ref Vector4 value ) => ImGui.InputFloat4( Name, ref value, format: FloatFormat );
    }
}
