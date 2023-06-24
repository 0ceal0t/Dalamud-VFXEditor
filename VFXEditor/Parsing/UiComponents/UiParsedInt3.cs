using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class UiParsedInt3 : IParsedUiBase {
        public readonly string Name;
        public readonly ParsedInt P1;
        public readonly ParsedInt P2;
        public readonly ParsedInt P3;

        private Vector3 Value => new( P1.Value, P2.Value, P3.Value );

        public UiParsedInt3( string name, ParsedInt p1, ParsedInt p2, ParsedInt p3 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public bool Draw( CommandManager manager ) {
            var ret = false;

            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector3s[Name] = Value;
            if( copy.IsPasting && copy.Vector3s.TryGetValue( Name, out var val ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedSimpleCommand<int>( P1, ( int )val.X ) );
                command.Add( new ParsedSimpleCommand<int>( P2, ( int )val.Y ) );
                command.Add( new ParsedSimpleCommand<int>( P3, ( int )val.Z ) );
                manager.Add( command );
                ret = true;
            }

            var value = Value;
            if( ImGui.InputFloat3( Name, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedSimpleCommand<int>( P1, ( int )value.X ) );
                command.Add( new ParsedSimpleCommand<int>( P2, ( int )value.Y ) );
                command.Add( new ParsedSimpleCommand<int>( P3, ( int )value.Z ) );
                manager.Add( command );
                ret = true;
            }

            return ret;
        }
    }
}
