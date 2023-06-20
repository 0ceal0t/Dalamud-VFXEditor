using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetString : ParsedString {
        public TmbOffsetString( string name ) : base( name ) { }

        public override void Read( TmbReader reader ) {
            Value = reader.ReadOffsetString();
        }

        public override void Write( TmbWriter writer ) {
            writer.WriteOffsetString( Value );
        }
    }
}
