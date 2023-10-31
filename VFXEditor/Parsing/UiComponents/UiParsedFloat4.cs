using ImGuiNET;
using System.Numerics;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Parsing {
    public class UiParsedFloat4 : IUiItem {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;
        public readonly ParsedFloat P3;
        public readonly ParsedFloat P4;

        private Vector4 Value => new( P1.Value, P2.Value, P3.Value, P4.Value );

        public UiParsedFloat4( string name, ParsedFloat p1, ParsedFloat p2, ParsedFloat p3, ParsedFloat p4 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        public void Draw() {
            // Copy/Paste
            /*
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.SetValue( this, Name, Value );
            if( copy.IsPasting && copy.GetValue<Vector4>( this, Name, out var val ) ) {
                var command = new CompoundCommand();
                command.Add( new ParsedSimpleCommand<float>( P1, val.X ) );
                command.Add( new ParsedSimpleCommand<float>( P2, val.Y ) );
                command.Add( new ParsedSimpleCommand<float>( P3, val.Z ) );
                command.Add( new ParsedSimpleCommand<float>( P4, val.W ) );
                manager.Add( command );
            }*/

            var value = Value;
            if( ImGui.InputFloat4( Name, ref value ) ) {
                var command = new CompoundCommand();
                command.Add( new ParsedSimpleCommand<float>( P1, value.X ) );
                command.Add( new ParsedSimpleCommand<float>( P2, value.Y ) );
                command.Add( new ParsedSimpleCommand<float>( P3, value.Z ) );
                command.Add( new ParsedSimpleCommand<float>( P4, value.W ) );
                CommandManager.Add( command );
            }
        }
    }
}
