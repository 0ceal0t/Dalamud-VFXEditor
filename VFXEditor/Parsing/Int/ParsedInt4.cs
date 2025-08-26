using Dalamud.Bindings.ImGui;
using System.IO;
using Int4 = SharpDX.Int4;

namespace VfxEditor.Parsing {
    public class ParsedInt4 : ParsedSimpleBase<Int4> {
        public ParsedInt4( string name, Int4 value ) : base( name, value ) { }

        public ParsedInt4( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadInt32();
            Value.Y = reader.ReadInt32();
            Value.Z = reader.ReadInt32();
            Value.W = reader.ReadInt32();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
            writer.Write( Value.W );
        }

        protected override void DrawBody() {
            var value = Value.ToArray();
            if( ImGui.InputInt( Name, value ) ) {
                Update( value );
            }
        }
    }
}