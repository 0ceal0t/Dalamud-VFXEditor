using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : ParsedBase {
        public readonly string Name;
        public Vector3 Value = new(0);

        public ParsedFloat3( string name, Vector3 defaultValue) : this( name ) {
            Value = defaultValue;
        }

        public ParsedFloat3( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
            Value.Z = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector3s[Name] = Value;
            if( copy.IsPasting && copy.Vector3s.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedFloat3Command( this, val ) );
            }

            var value = Value;
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                manager.Add( new ParsedFloat3Command( this, value ) );
            }
        }
    }
}
