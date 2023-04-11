using ImGuiNET;
using System;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedByteBool : ParsedSimpleBase<bool> {
        public readonly string Name;

        public ParsedByteBool( string name, bool defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedByteBool( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadBoolean();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value );
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
