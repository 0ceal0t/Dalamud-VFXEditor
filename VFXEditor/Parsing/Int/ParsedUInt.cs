using Dalamud.Bindings.ImGui;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedUInt : ParsedSimpleBase<uint> {
        private int Size;

        public ParsedUInt( string name, uint value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedUInt( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = Size switch {
                4 => reader.ReadUInt32(),
                2 => reader.ReadUInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            };
        }

        public override void Write( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( Value );
            else if( Size == 2 ) writer.Write( ( short )Value );
            else writer.Write( ( byte )Value );
        }

        protected override void DrawBody() {
            var value = ( int )Value;
            if( InTable ? ImGui.InputInt( Name, ref value, 0, 0 ) : ImGui.InputInt( Name, ref value ) ) {
                Update( ( uint )value );
            }
        }
    }
}
