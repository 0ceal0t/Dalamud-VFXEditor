using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Utils;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetFloat4 : ParsedFloat4 {
        public TmbOffsetFloat4( string name, Vector4 defaultValue ) : base( name, defaultValue ) { }

        public TmbOffsetFloat4( string name ) : base( name ) { }

        public override void Read( ParsingReader reader ) {
            if( reader is TmbReader tmbReader ) {
                Value = tmbReader.ReadOffsetVector4();
            }
        }

        public override void Write( ParsingWriter writer ) {
            if( writer is TmbWriter tmbWriter ) {
                tmbWriter.WriteExtraVector4( Value );
            }
        }
    }
}
