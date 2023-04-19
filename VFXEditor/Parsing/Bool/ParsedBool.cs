using ImGuiNET;
using System;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedBool : ParsedSimpleBase<bool> {
        public readonly string Name;
        private int Size;
        private int IntValue => Value ? 1 : 0;

        public ParsedBool( string name, bool defaultValue, int size = 4 ) : this( name, size ) {
            Value = defaultValue;
        }

        public ParsedBool( string name, int size = 4 ) {
            Name = name;
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = (Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            }) == 1;
        }

        public override void Write( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( IntValue );
            else if( Size == 2 ) writer.Write( ( short )IntValue );
            else writer.Write( ( byte )IntValue );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Bools[Name] = Value;
            if( copy.IsPasting && copy.Bools.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<bool>( this, val ) );
            }

            var value = Value;
            if( ImGui.Checkbox( Name + id, ref value ) ) manager.Add( new ParsedSimpleCommand<bool>( this, value ) );
        }
    }
}
