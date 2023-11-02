using ImGuiNET;
using OtterGui.Raii;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Int {
    public class ParsedUIntHex : ParsedUInt {
        public ParsedUIntHex( string name ) : base( name ) { }

        public ParsedUIntHex( string name, uint value ) : base( name, value ) { }

        protected override void DrawBody() {
            var xSize = ImGui.CalcTextSize( "0x" ).X + ImGui.GetStyle().FramePadding.X * 2 + 2;
            var inputSize = UiUtils.GetOffsetInputSize( xSize );

            using( var color1 = ImRaii.PushColor( ImGuiCol.ButtonHovered, ImGui.GetColorU32( ImGuiCol.MenuBarBg ) ) )
            using( var color2 = ImRaii.PushColor( ImGuiCol.ButtonActive, ImGui.GetColorU32( ImGuiCol.MenuBarBg ) ) )
            using( var color3 = ImRaii.PushColor( ImGuiCol.Button, ImGui.GetColorU32( ImGuiCol.MenuBarBg ) ) )
            using( var text = ImRaii.PushColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) ) )
            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 0 ) ) ) {
                ImGui.Button( "0x" );
                ImGui.SameLine();
            }

            var value = ( int )Value;
            ImGui.SetNextItemWidth( inputSize );
            if( ImGui.InputInt( Name, ref value, 0, 0, ImGuiInputTextFlags.CharsHexadecimal ) ) {
                SetValue( ( uint )value );
            }
        }
    }
}
