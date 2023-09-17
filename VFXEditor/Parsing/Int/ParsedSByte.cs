using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data;

namespace VfxEditor.Parsing {
    public class ParsedSByte : ParsedSimpleBase<sbyte, int> {
        public ParsedSByte( string name, sbyte value ) : this( name ) {
            Value = value;
        }

        public ParsedSByte( string name ) : base( name ) { }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadSByte();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value );
        }

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = ( int )Value;
            if( ImGui.InputInt( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<sbyte>( this, ( sbyte )value ) );
            }
        }

        protected override Dictionary<string, int> GetCopyMap( CopyManager manager ) => manager.Ints;

        protected override int ToCopy() => Value;

        protected override sbyte FromCopy( int val ) => ( sbyte )val;
    }
}

