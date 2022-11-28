using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat2 : ParsedBase {
        public readonly string Name;
        public Vector2 Value = new(0);

        public ParsedFloat2( string name, Vector2 defaultValue) : this( name ) {
            Value = defaultValue;
        }

        public ParsedFloat2( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector2s[Name] = Value;
            if( copy.IsPasting && copy.Vector2s.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedFloat2Command( this, val ) );
            }

            var value = Value;
            if( ImGui.InputFloat2( Name + id, ref value ) ) {
                manager.Add( new ParsedFloat2Command( this, value ) );
            }
        }
    }
}
