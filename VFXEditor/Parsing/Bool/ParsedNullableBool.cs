using ImGuiNET;
using System.IO;
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
            var value = Value == true;
            if( ImGui.Checkbox( Name, ref value ) ) {
                SetValue( value );
            }
        }
    }
}
