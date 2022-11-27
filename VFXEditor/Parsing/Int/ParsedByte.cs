using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedByte : ParsedInt {
        public ParsedByte( string name, int defaultValue ) : base( name, defaultValue: defaultValue, size: 1 ) { }
        public ParsedByte( string name ) : base( name, size: 1 ) { }
    }
}
