using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public class ParsedFloat : IParsedBase {
        public readonly string Name;
        public float Value = 0f;

        public ParsedFloat( string name ) {
            Name = name;
        }

        public void Read( BinaryReader reader, int _ ) {
            Value = reader.ReadSingle();
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Value );
        }

        public void Draw( string id, CommandManager manager ) {
            var value = Value;
            if( ImGui.InputFloat( Name + id, ref value ) ) {
                manager.Add( new ParsedFloatCommand( this, value ) );
            }
        }
    }
}
