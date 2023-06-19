using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedString : ParsedSimpleBase<string, string> {
        public readonly uint MaxSize;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private string StateBeforeEdit = "";

        public ParsedString( string name, string defaultValue, uint maxSize = 255 ) : this( name, maxSize ) {
            Value = defaultValue;
        }

        public ParsedString( string name, uint maxSize = 255 ) : base( name ) {
            Value = "";
            MaxSize = maxSize;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = FileUtils.ReadString( reader );
        }

        public override void Write( BinaryWriter writer ) => FileUtils.WriteString( writer, Value, writeNull: true );

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var prevValue = Value;
            if( ImGui.InputText( Name, ref Value, MaxSize ) ) {
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

        public void Pad( BinaryReader reader, int length ) => reader.ReadBytes( length - Value.Length - 1 );

        public void Pad( BinaryWriter writer, int length ) {
            for( var i = 0; i < ( length - Value.Length - 1 ); i++ ) writer.Write( ( byte )0 );
        }

        protected override Dictionary<string, string> GetCopyMap( CopyManager manager ) => manager.Strings;

        protected override string ToCopy() => Value;

        protected override string FromCopy( string val ) => val;
    }
}
