using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat4 : ParsedSimpleBase<Vector4> {
        private readonly int Size;

        public ParsedFloat4( string name, Vector4 value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedFloat4( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        private float ReadElement( BinaryReader reader ) => Size switch {
            8 => ( float )reader.ReadDouble(),
            _ => reader.ReadSingle()
        };

        private void WriteElement( BinaryWriter writer, float data ) {
            if( Size == 8 ) writer.Write( ( double )data );
            else writer.Write( data );
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = ReadElement( reader );
            Value.Y = ReadElement( reader );
            Value.Z = ReadElement( reader );
            Value.W = ReadElement( reader );
        }

        public override void Write( BinaryWriter writer ) {
            WriteElement( writer, Value.X );
            WriteElement( writer, Value.Y );
            WriteElement( writer, Value.Z );
            WriteElement( writer, Value.W );
        }

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat4( Name, ref value ) ) {
                Update( value );
            }
        }
    }
}
