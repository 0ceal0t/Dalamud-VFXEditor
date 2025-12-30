using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedDouble : ParsedSimpleBase<double> {
        public bool HighPrecision = true;
        public ParsedDouble( string name, double value ) : base( name, value ) { }

        public ParsedDouble( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int _ ) {
            Value = reader.ReadDouble();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputDouble( Name, ref value, 0.0f, 0.0f, format: HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f" ) ) {
                Update( value );
            }
        }
    }
}
