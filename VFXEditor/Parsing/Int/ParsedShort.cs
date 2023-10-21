namespace VfxEditor.Parsing {
    public class ParsedShort : ParsedInt {
        public ParsedShort( string name, int value ) : base( name, value, 2 ) { }

        public ParsedShort( string name ) : base( name, size: 2 ) { }
    }
}
