using Dalamud.Bindings.ImGui;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedSByte : ParsedSimpleBase<sbyte> {
        public ParsedSByte( string name, sbyte value ) : base( name, value ) { }

        public ParsedSByte( string name ) : base( name ) { }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadSByte();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value );
        }

        protected override void DrawBody() {
            var value = ( int )Value;
            if( ImGui.InputInt( Name, ref value ) ) {
                Update( ( sbyte )value );
            }
        }
    }
}

