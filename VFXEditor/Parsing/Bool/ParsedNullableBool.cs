using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedNullableBool : ParsedSimpleBase<bool?, bool> {
        private int Size;

        public ParsedNullableBool( string name, bool defaultValue, int size = 4 ) : this( name, size ) {
            Value = defaultValue;
        }

        public ParsedNullableBool( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            var b = reader.ReadByte();
            Value = b switch {
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

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value == true;
            if( ImGui.Checkbox( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<bool?>( this, value ) );
            }
        }

        protected override Dictionary<string, bool> GetCopyMap( CopyManager manager ) => manager.Bools;

        protected override bool ToCopy() => Value == true;

        protected override bool? FromCopy( bool val ) => val;
    }
}
