using ImGuiNET;
using OtterGui.Raii;
using System;
using System.IO;
using System.Numerics;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Parsing {
    public class ParsedNullableBool : ParsedSimpleBase<bool?> {
        private int Size;

        public ParsedNullableBool( string name, bool value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedNullableBool( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            var value = reader.ReadByte();
            Value = value switch {
                0x00 => false,
                0x01 => true,
                0xff => null,
                _ => null
            };
            Size = size;
        }

        public override void Write( BinaryWriter writer ) {
            byte v = Value switch {
                true => 0x01,
                false => 0x00,
                null => 0xff
            };
            writer.Write( v );
            AvfxBase.WritePad( writer, Size - 1 );
        }

        protected override void DrawBody() {
            using var _ = ImRaii.PushId( Name );

            if( InTable ) ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( ImGui.GetContentRegionAvail().X - ImGui.GetFrameHeight() ) / 2f );
            var pos = ImGui.GetCursorScreenPos();

            var prevValue = Value;
            var value = Value == true;
            if( ImGui.Checkbox( Name, ref value ) ) {
                if( prevValue == null ) Update( true ); // null -> true
                else if( prevValue == true ) Update( false ); // true -> false
                else if( prevValue == false ) Update( null ); // false -> null
            }

            // https://github.com/ocornut/imgui/blob/master/imgui_widgets.cpp#L1134
            if( Value != null ) return;
            var color = ImGui.GetColorU32( ImGuiCol.CheckMark );
            var rounding = ImGui.GetStyle().FrameRounding;
            var size = ImGui.GetFrameHeight();
            var pad = new Vector2( ( float )Math.Floor( size / 6f ), size / 2.5f );

            ImGui.GetWindowDrawList().AddRectFilled( pos + pad, pos + new Vector2( size, size ) - pad, color, 0 );
        }
    }
}
