using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class UiParsedFloat3 : IParsedUiBase {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;
        public readonly ParsedFloat P3;

        public Vector3 Value => new( P1.Value, P2.Value, P3.Value );

        public UiParsedFloat3( string name, ParsedFloat p1, ParsedFloat p2, ParsedFloat p3 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public void Draw( CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector3s[Name] = Value;
            if( copy.IsPasting && copy.Vector3s.TryGetValue( Name, out var val ) ) {
                var command = new CompoundCommand();
                command.Add( new ParsedSimpleCommand<float>( P1, val.X ) );
                command.Add( new ParsedSimpleCommand<float>( P2, val.Y ) );
                command.Add( new ParsedSimpleCommand<float>( P3, val.Z ) );
                manager.Add( command );
            }

            var value = Value;
            if( ImGui.InputFloat3( Name, ref value ) ) {
                var command = new CompoundCommand();
                command.Add( new ParsedSimpleCommand<float>( P1, value.X ) );
                command.Add( new ParsedSimpleCommand<float>( P2, value.Y ) );
                command.Add( new ParsedSimpleCommand<float>( P3, value.Z ) );
                manager.Add( command );
            }
        }
    }
}
