using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetString : ParsedString {
        public TmbOffsetString( string name, string defaultValue ) : base( name, defaultValue ) { }

        public TmbOffsetString( string name, uint maxSize = 255 ) : base( name, maxSize ) { }

        public override void Read( TmbReader reader ) {
            Value = reader.ReadOffsetString();
        }

        public override void Write( TmbWriter writer ) {
            writer.WriteOffsetString( Value );
        }
    }
}
