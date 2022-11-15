using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedShort : ParsedInt {
        public ParsedShort( string name ) : base( name, size: 2 ) { }
    }
}
