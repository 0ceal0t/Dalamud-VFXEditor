using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public class UiParsedFloat3 : IParsedUiBase {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;
        public readonly ParsedFloat P3;

        public UiParsedFloat3( string name, ParsedFloat p1, ParsedFloat p2, ParsedFloat p3 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public void Draw( string id, CommandManager manager ) {
            var value = new Vector3( P1.Value, P2.Value, P3.Value );
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedFloatCommand( P1, value.X ) );
                command.Add( new ParsedFloatCommand( P2, value.Y ) );
                command.Add( new ParsedFloatCommand( P3, value.Z ) );
                manager.Add( command );
            }
        }
    }
}
