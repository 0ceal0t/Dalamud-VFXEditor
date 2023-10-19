using ImGuiNET;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : ParsedSimpleBase<Vector3> {
        public ParsedFloat3( string name, Vector3 value ) : this( name ) {
            Value = value;
        }

        public ParsedFloat3( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = reader.ReadSingle();
            Value.Y = reader.ReadSingle();
            Value.Z = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.X );
            writer.Write( Value.Y );
            writer.Write( Value.Z );
        }

        public override void Draw( CommandManager manager ) => Draw( manager, out var _ );

        public void Draw( CommandManager manager, out bool edited ) {
            edited = CopyPaste( manager );

            var value = Value;
            if( ImGui.InputFloat3( Name, ref value ) ) {
                manager.Add( new ParsedSimpleCommand<Vector3>( this, value ) );
                edited = true;
            }
        }
    }
}
