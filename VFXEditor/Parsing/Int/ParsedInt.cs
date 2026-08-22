using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Parsing.Utils;

namespace VfxEditor.Parsing {
    public class ParsedInt : ParsedSimpleBase<int> {
        private int Size;

        public ParsedInt( string name, int value, int size = 4 ) : base( name, value ) {
            Size = size;
        }

        public ParsedInt( string name, int size = 4 ) : base( name ) {
            Size = size;
        }

        public override void Read( BinaryReader reader ) => Read( reader, Size );

        public override void Read( BinaryReader reader, int size ) {
            Size = size;
            Value = ( int )SizedNumberUtils.ReadSigned( reader, Size );
        }

        public override void Write( BinaryWriter writer ) => SizedNumberUtils.WriteSigned( writer, Value, Size );

        protected override void DrawBody() {
            var value = Value;
            if( InTable ? ImGui.InputInt( Name, ref value, 0, 0 ) : ImGui.InputInt( Name, ref value, step: 1 ) ) Update( value );
        }
    }
}
