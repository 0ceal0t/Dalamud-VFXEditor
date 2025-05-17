using ImGuiNET;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedFloat3 : ParsedSimpleBase<Vector3> {
        public bool HighPrecision = true;

        public ParsedFloat3( string name, Vector3 value ) : base( name, value ) { }

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

        protected override void DrawBody() {
            var value = Value;
            if( ImGui.InputFloat3( Name, ref value, HighPrecision ? UiUtils.HIGH_PRECISION_FORMAT : "%.3f" ) ) Update( value );
        }
    }
}
