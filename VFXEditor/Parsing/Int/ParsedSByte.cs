using ImGuiNET;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedSByte : ParsedSimpleBase<sbyte> {
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
            CopyPaste( manager );

            var value = ( int )Value;
            if( ImGui.InputInt( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<sbyte>( this, ( sbyte )value ) );
            }
        }
    }
}

