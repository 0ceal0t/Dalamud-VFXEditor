using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Parsing {
    public class UiParsedInt3 : IParsedUiBase {
        public readonly string Name;
        public readonly ParsedInt P1;
        public readonly ParsedInt P2;
        public readonly ParsedInt P3;

        public UiParsedInt3( string name, ParsedInt p1, ParsedInt p2, ParsedInt p3 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public void Draw( string id, CommandManager manager ) {
            var value = new Vector3( P1.Value, P2.Value, P3.Value );
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedIntCommand( P1, ( int )value.X ) );
                command.Add( new ParsedIntCommand( P2, ( int )value.Y ) );
                command.Add( new ParsedIntCommand( P3, ( int )value.Z ) );
                manager.Add( command );
            }
        }
    }
}
