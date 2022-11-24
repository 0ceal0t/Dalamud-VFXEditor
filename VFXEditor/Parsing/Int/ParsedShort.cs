using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedShort : ParsedInt {
        public ParsedShort( string name, int defaultValue ) : base( name, defaultValue: defaultValue, size: 2 ) { }
        public ParsedShort( string name ) : base( name, size: 2 ) { }
    }
}
