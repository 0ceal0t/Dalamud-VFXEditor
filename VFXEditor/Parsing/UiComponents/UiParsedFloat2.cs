using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public class UiParsedFloat2 : IParsedUiBase {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;

        public UiParsedFloat2( string name, ParsedFloat p1, ParsedFloat p2 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
        }

        public void Draw( string id, CommandManager manager ) {
            var value = new Vector2( P1.Value, P2.Value );
            if( ImGui.InputFloat2( Name + id, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedFloatCommand( P1, value.X ) );
                command.Add( new ParsedFloatCommand( P2, value.Y ) );
                manager.Add( command );
            }
        }
    }
}
