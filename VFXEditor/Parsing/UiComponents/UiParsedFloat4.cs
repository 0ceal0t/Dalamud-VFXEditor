using ImGuiNET;
using System.Numerics;
using VfxEditor.Data.Copy;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class UiParsedFloat4 : IUiItem {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;
        public readonly ParsedFloat P3;
        public readonly ParsedFloat P4;
        public bool HighPrecision = true;

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
            CopyManager.TrySetValue( this, Name, Value );
            if( CopyManager.TryGetValue<Vector4>( this, Name, out var val ) ) {
                CommandManager.Paste( new CompoundCommand( new[] {
                     new ParsedSimpleCommand<float>( P1, val.X ),
                     new ParsedSimpleCommand<float>( P2, val.Y ),
                     new ParsedSimpleCommand<float>( P3, val.Z ),
                     new ParsedSimpleCommand<float>( P4, val.W )
                } ) );
            }

            var value = Value;
            if( ImGui.InputFloat4( Name, ref value, HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f" ) ) {
                CommandManager.Add( new CompoundCommand( new[] {
                     new ParsedSimpleCommand<float>( P1, value.X ),
                     new ParsedSimpleCommand<float>( P2, value.Y ),
                     new ParsedSimpleCommand<float>( P3, value.Z ),
                     new ParsedSimpleCommand<float>( P4, value.W )
                } ) );
            }
        }
    }
}
