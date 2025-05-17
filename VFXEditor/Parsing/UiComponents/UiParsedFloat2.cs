using ImGuiNET;
using System.Numerics;
using VfxEditor.Data.Copy;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class UiParsedFloat2 : IUiItem {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;
        public bool HighPrecision = true;

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
                CommandManager.Paste( new CompoundCommand( new[] {
                    new ParsedSimpleCommand<float>( P1, val.X ),
                    new ParsedSimpleCommand<float>( P2, val.Y )
                } ) );
            }

            var value = Value;
            if( ImGui.InputFloat2( Name, ref value, HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f" ) ) {
                CommandManager.Add( new CompoundCommand( new[] {
                    new ParsedSimpleCommand<float>( P1, value.X ),
                    new ParsedSimpleCommand<float>( P2, value.Y )
                } ) );
            }
        }
    }
}
