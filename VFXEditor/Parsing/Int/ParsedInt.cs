using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedInt : IParsedBase {
        public readonly string Name;
        private int Size;
        public int Value = 0;

        public ParsedInt( string name, int size = 4 ) {
            Name = name;
            Size = size;
        }

        public void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            };
        }

        public void Write( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( Value );
            else if( Size == 2 ) writer.Write( ( short )Value );
            else writer.Write( ( byte )Value );
        }

        public void Draw( string id, CommandManager manager ) {
            var value = Value;
            if( ImGui.InputInt( Name + id, ref value ) ) {
                manager.Add( new ParsedIntCommand( this, value ) );
            }
        }
    }
}
