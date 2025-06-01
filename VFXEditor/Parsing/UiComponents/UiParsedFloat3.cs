using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Data.Copy;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class UiParsedFloat3 : IUiItem {
        public readonly string Name;
        public readonly ParsedFloat P1;
        public readonly ParsedFloat P2;
        public readonly ParsedFloat P3;
        public bool HighPrecision = true;

        public Vector3 Value => new( P1.Value, P2.Value, P3.Value );

        public UiParsedFloat3( string name, ParsedFloat p1, ParsedFloat p2, ParsedFloat p3 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public void Draw() {
            // Copy/Paste
            CopyManager.TrySetValue( this, Name, Value );
            if( CopyManager.TryGetValue<Vector3>( this, Name, out var val ) ) {
                var commands = new List<ICommand> {
                    new ParsedSimpleCommand<float>( P1, val.X ),
                    new ParsedSimpleCommand<float>( P2, val.Y ),
                    new ParsedSimpleCommand<float>( P3, val.Z )
                };
                CommandManager.Paste( new CompoundCommand( commands ) );
            }

            var value = Value;
            if( ImGui.InputFloat3( Name, ref value, HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f" ) ) {
                var commands = new List<ICommand> {
                    new ParsedSimpleCommand<float>( P1, value.X ),
                    new ParsedSimpleCommand<float>( P2, value.Y ),
                    new ParsedSimpleCommand<float>( P3, value.Z )
                };
                CommandManager.Add( new CompoundCommand( commands ) );
            }
        }
    }
}
