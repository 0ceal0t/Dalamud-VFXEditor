using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat2 : ParsedSimpleBase<Vector2> {
        public readonly string Name;

        public ParsedFloat2( string name, Vector2 defaultValue ) : this( name ) {
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

        public override void Draw( CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector2s[Name] = Value;
            if( copy.IsPasting && copy.Vector2s.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<Vector2>( this, val ) );
            }

            var value = Value;
            if( ImGui.InputFloat2( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<Vector2>( this, value ) );
            }
        }
    }
}
