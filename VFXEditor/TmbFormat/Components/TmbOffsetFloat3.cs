using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetFloat3 : ParsedFloat3 {
        public TmbOffsetFloat3( string name, Vector3 defaultValue ) : base( name, defaultValue ) { }

        public TmbOffsetFloat3( string name ) : base( name ) { }

        public override void Read( TmbReader reader ) {
            Value = reader.ReadOffsetVector3();
        }

        public override void Write( TmbWriter writer ) {
            writer.WriteExtraVector3( Value );
        }
    }
}
