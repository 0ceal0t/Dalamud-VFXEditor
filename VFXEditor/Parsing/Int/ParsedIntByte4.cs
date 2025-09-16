using Dalamud.Bindings.ImGui;
using System;
using System.IO;
using System.Linq;

namespace VfxEditor.Parsing.Int {
    public class ParsedIntByte4 : ParsedSimpleBase<int> {
        public ParsedIntByte4( string name, int value ) : base( name, value ) { }

        public ParsedIntByte4( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = reader.ReadInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override void DrawBody() {
            var bytes = BitConverter.GetBytes( Value );
            var value = bytes.Select( x => ( int )x ).ToArray();

            if( ImGui.InputInt( Name, value ) ) {
                var newValue = BitConverter.ToInt32( value.Select( x => ( byte )x ).ToArray() );
                Update( newValue );
            }
        }
    }
}
