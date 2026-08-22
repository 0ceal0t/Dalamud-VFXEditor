using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Parsing.Utils;

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
            Value = SizedNumberUtils.ReadSigned( reader, Size ) == 1;
        }

        public override void Write( BinaryWriter writer ) => SizedNumberUtils.WriteSigned( writer, IntValue, Size );

        protected override void DrawBody() {
            var value = Value;
            if( InTable ) ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( ImGui.GetContentRegionAvail().X - ImGui.GetFrameHeight() ) / 2f );
            if( ImGui.Checkbox( Name, ref value ) ) {
                Update( value );
            }
        }
    }
}
