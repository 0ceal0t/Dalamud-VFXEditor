using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Parsing.Utils;

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
            Value = ( uint )SizedNumberUtils.ReadUnsigned( reader, Size );
        }

        public override void Write( BinaryWriter writer ) => SizedNumberUtils.WriteUnsigned( writer, Value, Size );

        protected override void DrawBody() {
            var value = ( int )Value;
            if( InTable ? ImGui.InputInt( Name, ref value, 0, 0 ) : ImGui.InputInt( Name, ref value ) ) {
                Update( ( uint )value );
            }
        }
    }
}
