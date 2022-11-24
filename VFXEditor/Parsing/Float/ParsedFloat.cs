using ImGuiNET;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedFloat : ParsedBase {
        public readonly string Name;
        public float Value = 0f;

        public ParsedFloat( string name, float defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedFloat( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Floats[Name] = Value;
            if( copy.IsPasting && copy.Floats.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedFloatCommand( this, val ) );
            }

            var value = Value;
            if( ImGui.InputFloat( Name + id, ref value ) ) {
                manager.Add( new ParsedFloatCommand( this, value ) );
            }
        }
    }
}
