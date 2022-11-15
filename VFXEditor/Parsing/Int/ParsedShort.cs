using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedShort : ParsedInt {
        public ParsedShort( string name ) : base( name, size: 2 ) { }

        public void Read( BinaryReader reader ) => Read( reader, 2 );
    }
}
