using ImGuiNET;
using System;
using System.Numerics;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedIntColor : IParsedBase {
        public readonly string Name;
        public Vector4 Value = new(0);

        private bool ColorDrag = false;
        private DateTime ColorDragTime = DateTime.Now;
        private Vector4 ColorDragState;

        public ParsedIntColor( string name ) {
            Name = name;
        }

        public void Read( BinaryReader reader, int size ) {
            Value.X = reader.ReadByte() / 255f;
            Value.Y = reader.ReadByte() / 255f;
            Value.Z = reader.ReadByte() / 255f;
            Value.W = reader.ReadByte() / 255f;
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( ( byte )( int )( Value.X * 255f ) );
            writer.Write( ( byte )( int )( Value.Y * 255f ) );
            writer.Write( ( byte )( int )( Value.Z * 255f ) );
            writer.Write( ( byte )( int )( Value.W * 255f ) );
        }

        public void Draw( string id, CommandManager manager ) {
            var prevValue = Value;
            if( ImGui.ColorEdit4( Name + id, ref Value, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoDragDrop ) ) {
                if( !ColorDrag ) {
                    ColorDrag = true;
                    ColorDragState = prevValue;
                }
                ColorDragTime = DateTime.Now;
            }
            else if( ColorDrag && ( DateTime.Now - ColorDragTime ).TotalMilliseconds > 200 ) {
                ColorDrag = false;
                manager.Add( new ParsedIntColorCommand( this, ColorDragState ) );
            }
        }
    }
}
