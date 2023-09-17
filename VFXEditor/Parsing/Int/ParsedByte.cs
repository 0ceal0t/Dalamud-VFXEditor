namespace VfxEditor.Parsing {
    public class ParsedByte : ParsedInt {
        public ParsedByte( string name, int value ) : base( name, value: value, size: 1 ) { }

        public ParsedByte( string name ) : base( name, size: 1 ) { }
    }
}
