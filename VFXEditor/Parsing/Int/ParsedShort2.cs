using ImGuiNET;
using System.IO;
using Int4 = SharpDX.Int4;

namespace VfxEditor.Parsing.Int {
    public class ParsedShort2 : ParsedInt4 {
        public ParsedShort2( string name, Int4 value ) : this( name ) {
            Value = value;
        }

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

        public override void Draw( CommandManager manager ) {
            Copy( manager );

            var value = Value.ToArray();
            if( ImGui.InputInt2( Name, ref value[0] ) ) {
                manager.Add( new ParsedSimpleCommand<Int4>( this, value ) );
            }
        }
    }
}
