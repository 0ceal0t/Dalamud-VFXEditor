using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedString : ParsedSimpleBase<string, string> {
        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private string StateBeforeEdit = "";

        public ParsedString( string name, string defaultValue ) : this( name ) {
            Value = defaultValue;
        }

        public ParsedString( string name ) : base( name ) {
            Value = "";
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = FileUtils.ReadString( reader );
        }

        public override void Write( BinaryWriter writer ) => FileUtils.WriteString( writer, Value, writeNull: true );

        public override void Draw( CommandManager manager ) => Draw( manager, 255, Name, ImGuiInputTextFlags.None );

        public void Draw( CommandManager manager, uint maxSize, string label, ImGuiInputTextFlags flags ) {
            Copy( manager );

            var prevValue = Value;
            if( ImGui.InputText( label, ref Value, maxSize, flags ) ) {
                if( !Editing ) {
                    Editing = true;
                    StateBeforeEdit = prevValue;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;
                manager.Add( new ParsedSimpleCommand<string>( this, StateBeforeEdit, Value ) );
            }
        }

        protected override Dictionary<string, string> GetCopyMap( CopyManager manager ) => manager.Strings;

        protected override string ToCopy() => Value;

        protected override string FromCopy( string val ) => val;
    }
}
