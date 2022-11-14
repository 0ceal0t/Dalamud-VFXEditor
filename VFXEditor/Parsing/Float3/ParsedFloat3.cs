using ImGuiNET;
using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : IParsedBase {
        public readonly string Name;
        public Vector3 Value = new(0);

        public ParsedFloat3( string name ) {
            Name = name;
        }

        public void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
            Value.Z = reader.ReadSingle();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
        }

        public void Draw( string id, CommandManager manager ) {
            var value = Value;
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                manager.Add( new ParsedFloat3Command( this, value ) );
            }
        }
    }
}
