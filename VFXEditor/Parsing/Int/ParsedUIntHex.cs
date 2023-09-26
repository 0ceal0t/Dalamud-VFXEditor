using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Int {
    public class ParsedUIntHex : ParsedUInt {
        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private string HexString = "";

        public ParsedUIntHex( string name ) : base( name ) { }

        public ParsedUIntHex( string name, uint value ) : base( name, value ) { }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            using var _ = ImRaii.PushId( Name );

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

            if( !Editing ) HexString = $"{Value:X4}";

            ImGui.SetNextItemWidth( inputSize );
            if( ImGui.InputTextWithHint( Name, "Hex Value", ref HexString, 255 ) ) {
                if( !Editing ) {
                    Editing = true;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;

                try {
                    var newValue = string.IsNullOrEmpty( HexString ) ? 0 : Convert.ToUInt32( HexString, 16 ); // Try to convert it back
                    manager.Add( new ParsedSimpleCommand<uint>( this, newValue ) );
                }
                catch( Exception ) { }
            }
        }
    }
}
