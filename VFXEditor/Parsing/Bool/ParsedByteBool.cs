namespace VfxEditor.Parsing {
    public class ParsedByteBool : ParsedBool {
        public ParsedByteBool( string name, bool value ) : base( name, value: value, size: 1 ) { }

        public ParsedByteBool( string name ) : base( name, size: 1 ) { }
    }
}
