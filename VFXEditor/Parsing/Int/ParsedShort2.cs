using ImGuiNET;
using System.IO;
using Int4 = SharpDX.Int4;

namespace VfxEditor.Parsing.Int {
    public class ParsedShort2 : ParsedInt4 {
        public ParsedShort2( string name, Int4 value ) : base( name, value ) { }

        public ParsedShort2( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadInt16();
            Value.Y = reader.ReadInt16();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( short )Value.X );
            writer.Write( ( short )Value.Y );
        }

        protected override void DrawBody() {
            var value = Value.ToArray();
            if( ImGui.InputInt2( Name, ref value[0] ) ) {
                SetValue( value );
            }
        }
    }
}
