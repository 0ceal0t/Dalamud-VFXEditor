using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : ParsedSimpleBase<Vector3> {
        private readonly int Size;

        public ParsedFloat3( string name, Vector3 value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedFloat3( string name, int size = 4 ) : base( name ) {
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
        }

        public override void Write( BinaryWriter writer ) {
            WriteElement( writer, Value.X );
            WriteElement( writer, Value.Y );
            WriteElement( writer, Value.Z );
        }

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat3( Name, ref value ) ) Update( value );
        }
    }
}
