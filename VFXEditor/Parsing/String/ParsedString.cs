using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedString : ParsedBase {
        public readonly string Name;
        public readonly uint MaxSize;
        public string Value = "";

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private string StateBeforeEdit = "";

        public ParsedString( string name, string defaultValue, uint maxSize = 255 ) : this( name, maxSize ) {
            Value = defaultValue;
        }

        public ParsedString( string name, uint maxSize = 255 ) {
            Name = name;
            MaxSize = maxSize;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = FileUtils.ReadString( reader );
        }

        public override void Write( BinaryWriter writer ) {
            FileUtils.WriteString( writer, Value, writeNull: true );
        }

        public override void Draw( string id, CommandManager manager ) {
            // Copy/Paste
            var copy = manager.Copy;
            if( copy.IsCopying ) copy.Strings[Name] = Value;
            if( copy.IsPasting && copy.Strings.TryGetValue( Name, out var val ) ) {
                copy.PasteCommand.Add( new ParsedStringCommand( this, val, Value ) );
            }

            var prevValue = Value;
            if( ImGui.InputText( Name + id, ref Value, MaxSize ) ) {
                if( !Editing ) {
                    Editing = true;
                    StateBeforeEdit = prevValue;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;
                manager.Add( new ParsedStringCommand( this, Value, StateBeforeEdit ) );
            }
        }
    }
}
