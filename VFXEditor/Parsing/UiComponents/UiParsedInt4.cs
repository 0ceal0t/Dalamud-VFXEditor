using ImGuiNET;
using System;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class UiParsedInt4 : IParsedUiBase {
        public readonly string Name;
        public readonly ParsedInt P1;
        public readonly ParsedInt P2;
        public readonly ParsedInt P3;
        public readonly ParsedInt P4;

        private Vector4 Value => new( P1.Value, P2.Value, P3.Value, P4.Value );

        public UiParsedInt4( string name, ParsedInt p1, ParsedInt p2, ParsedInt p3, ParsedInt p4 ) {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }

        public void Draw( CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector4s[Name] = Value;
            if( copy.IsPasting && copy.Vector4s.TryGetValue( Name, out var val ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedSimpleCommand<int>( P1, ( int )val.X ) );
                command.Add( new ParsedSimpleCommand<int>( P2, ( int )val.Y ) );
                command.Add( new ParsedSimpleCommand<int>( P3, ( int )val.Z ) );
                command.Add( new ParsedSimpleCommand<int>( P4, ( int )val.W ) );
                manager.Add( command );
            }

            var value = Value;
            if( ImGui.InputFloat4( Name, ref value ) ) {
                var command = new CompoundCommand( false, true );
                command.Add( new ParsedSimpleCommand<int>( P1, ( int )value.X ) );
                command.Add( new ParsedSimpleCommand<int>( P2, ( int )value.Y ) );
                command.Add( new ParsedSimpleCommand<int>( P3, ( int )value.Z ) );
                command.Add( new ParsedSimpleCommand<int>( P4, ( int )value.W ) );
                manager.Add( command );
            }
        }
    }
}
