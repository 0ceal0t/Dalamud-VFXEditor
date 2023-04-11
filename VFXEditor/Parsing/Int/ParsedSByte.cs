using ImGuiNET;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedSByte : ParsedSimpleBase<sbyte> {
        public readonly string Name;

        public ParsedSByte( string name, sbyte defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedSByte( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadSByte();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Ints[Name] = ( int )Value;
            if( copy.IsPasting && copy.Ints.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<sbyte>( this, ( sbyte )val ) );
            }

            var value = ( int )Value;
            if( ImGui.InputInt( Name + id, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<sbyte>( this, ( sbyte )value ) );
            }
        }
    }
}

