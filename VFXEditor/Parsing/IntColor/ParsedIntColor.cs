using ImGuiNET;
using System;
using System.Numerics;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedIntColor : ParsedBase {
        public readonly string Name;
        public Vector4 Value = new(0);

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private Vector4 StateBeforeEdit;

        public ParsedIntColor( string name, Vector4 defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedIntColor( string name ) {
            Name = name;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value.X = reader.ReadByte() / 255f;
            Value.Y = reader.ReadByte() / 255f;
            Value.Z = reader.ReadByte() / 255f;
            Value.W = reader.ReadByte() / 255f;
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( byte )( int )( Value.X * 255f ) );
            writer.Write( ( byte )( int )( Value.Y * 255f ) );
            writer.Write( ( byte )( int )( Value.Z * 255f ) );
            writer.Write( ( byte )( int )( Value.W * 255f ) );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Vector4s[Name] = Value;
            if( copy.IsPasting && copy.Vector4s.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedIntColorCommand( this, val, Value ) );
            }

            var prevValue = Value;
            if( ImGui.ColorEdit4( Name + id, ref Value, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoDragDrop ) ) {
                if( !Editing ) {
                    Editing = true;
                    StateBeforeEdit = prevValue;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;
                manager.Add( new ParsedIntColorCommand( this, Value, StateBeforeEdit ) );
            }
        }
    }
}
