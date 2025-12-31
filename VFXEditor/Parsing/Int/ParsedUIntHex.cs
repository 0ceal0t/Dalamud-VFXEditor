using Dalamud.Bindings.ImGui;
using System;

namespace VfxEditor.Parsing.Int {
    public class ParsedUIntHex : ParsedUInt {
        public ParsedUIntHex( string name ) : base( name ) { }

        public ParsedUIntHex( string name, uint value ) : base( name, value ) { }

        protected override void DrawBody() {
            var value = $"{Value:X8}";
            if( ImGui.InputText( Name, ref value, 256, ImGuiInputTextFlags.CharsHexadecimal )) {
                Update( Convert.ToUInt32( $"0x{value}", 16 ) );
            }
        }
    }
}