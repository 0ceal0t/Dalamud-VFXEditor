using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat4 : ParsedSimpleBase<Vector4> {
        public readonly string Name;

        public ParsedFloat4( string name, Vector4 defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedFloat4( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
            Value.Z = reader.ReadSingle();
            Value.W = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
            writer.Write( Value.W );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector4s[Name] = Value;
            if( copy.IsPasting && copy.Vector4s.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedSimpleCommand<Vector4>( this, val ) );
            }

            var value = Value;
            if( ImGui.InputFloat4( Name + id, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<Vector4>( this, value ) );
            }
        }
    }
}
