namespace VfxEditor.Parsing.Int {
    public class ParsedByte : ParsedInt {
        public ParsedByte( string name, int value ) : base( name, value, 1 ) { }

        public ParsedByte( string name ) : base( name, size: 1 ) { }
    }
}