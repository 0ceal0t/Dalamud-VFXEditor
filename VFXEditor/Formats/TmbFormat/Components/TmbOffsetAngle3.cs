using System.Numerics;
using VfxEditor.Formats.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.TmbFormat.Components {
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
