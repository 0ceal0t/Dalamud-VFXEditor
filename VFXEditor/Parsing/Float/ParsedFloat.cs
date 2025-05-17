using ImGuiNET;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedFloat : ParsedSimpleBase<float> {
        public bool HighPrecision = true;
        public ParsedFloat( string name, float value ) : base( name, value ) { }

        public ParsedFloat( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value = reader.ReadSingle();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat( Name, ref value, 0.0f, 0.0f, HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f") ) {
                Update( value );
            }
        }
    }
}
