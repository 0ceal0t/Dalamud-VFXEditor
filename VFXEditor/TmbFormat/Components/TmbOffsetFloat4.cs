using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetFloat4 : ParsedFloat4 {
        public TmbOffsetFloat4( string name, Vector4 defaultValue ) : base( name, defaultValue ) { }

        public TmbOffsetFloat4( string name ) : base( name ) { }

        public override void Read( TmbReader reader ) {
            Value = reader.ReadOffsetVector4();
        }

        public override void Write( TmbWriter writer ) {
            writer.WriteExtraVector4( Value );
        }
    }
}
