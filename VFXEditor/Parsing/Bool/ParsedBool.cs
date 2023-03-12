using ImGuiNET;
using System;
using System.IO;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Parsing {
    public class ParsedBool : ParsedBase {
        public readonly string Name;
        private int Size;
        public bool? Value = false;

        public ParsedBool( string name, bool defaultValue, int size = 4 ) : this( name, size ) {
            Value = defaultValue;
        }

        public ParsedBool( string name, int size = 4 ) {
            Name = name;
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

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Bools[Name] = Value == true;
            if( copy.IsPasting && copy.Bools.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedBoolCommand( this, val ) );
            }

            var value = Value == true;
            if( ImGui.Checkbox( Name + id, ref value ) ) manager.Add( new ParsedBoolCommand( this, value ) );
        }
    }
}
