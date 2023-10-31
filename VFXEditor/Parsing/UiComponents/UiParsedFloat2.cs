using ImGuiNET;
using System.Numerics;
using VfxEditor.Data.Copy;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Parsing {
    public class UiParsedFloat2 : IUiItem {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;

        private Vector2 Value => new( P1.Value, P2.Value );

        public UiParsedFloat2( string name, ParsedFloat p1, ParsedFloat p2 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
        }

        public void Draw() {
            // Copy/Paste
            CopyManager.TrySetValue( this, Name, Value );
            if( CopyManager.TryGetValue<Vector2>( this, Name, out var val ) ) {
                var command = new CompoundCommand();
                command.Add( new ParsedSimpleCommand<float>( P1, val.X ) );
                command.Add( new ParsedSimpleCommand<float>( P2, val.Y ) );
                CommandManager.Paste( command );
            }

            var value = Value;
            if( ImGui.InputFloat2( Name, ref value ) ) {
                var command = new CompoundCommand();
                command.Add( new ParsedSimpleCommand<float>( P1, value.X ) );
                command.Add( new ParsedSimpleCommand<float>( P2, value.Y ) );
                CommandManager.Add( command );
            }
        }
    }
}
