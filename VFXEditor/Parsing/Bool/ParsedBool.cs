using Dalamud.Bindings.ImGui;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedBool : ParsedSimpleBase<bool> {
        private int Size;
        private int IntValue => Value ? 1 : 0;

        public ParsedBool( string name, bool value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedBool( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = ( Size switch {
                4 => reader.ReadInt32(),
                2 => reader.ReadInt16(),
                1 => reader.ReadByte(),
                _ => reader.ReadByte()
            } ) == 1;
        }

        public override void Write( BinaryWriter writer ) {
            if( Size == 4 ) writer.Write( IntValue );
            else if( Size == 2 ) writer.Write( ( short )IntValue );
            else writer.Write( ( byte )IntValue );
        }

        protected override void DrawBody() {
            var value = Value;
            if( InTable ) ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( ImGui.GetContentRegionAvail().X - ImGui.GetFrameHeight() ) / 2f );
            if( ImGui.Checkbox( Name, ref value ) ) {
                Update( value );
            }
        }
    }
}
