using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Utils;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetFloat3 : ParsedFloat3 {
        public TmbOffsetFloat3( string name, Vector3 defaultValue ) : base( name, defaultValue ) { }

        public TmbOffsetFloat3( string name ) : base( name ) { }

        public override void Read( ParsingReader reader ) {
            if( reader is TmbReader tmbReader ) {
                Value = tmbReader.ReadOffsetVector3();
            }
        }

        public override void Write( ParsingWriter writer ) {
            if( writer is TmbWriter tmbWriter ) {
                tmbWriter.WriteExtraVector3( Value );
            }
        }
    }
}
