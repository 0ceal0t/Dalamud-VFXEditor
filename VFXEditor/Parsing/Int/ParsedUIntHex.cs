using Dalamud.Bindings.ImGui;

namespace VfxEditor.Parsing.Int {
    public class ParsedUIntHex : ParsedUInt {
        public ParsedUIntHex( string name ) : base( name ) { }

        public ParsedUIntHex( string name, uint value ) : base( name, value ) { }

        protected override void DrawBody() {
            var value = ( int )Value;
            if( ImGui.InputInt( Name, ref value, 0, 0, ImGuiInputTextFlags.CharsHexadecimal ) ) {
                Update( ( uint )value );
            }
        }
    }
}