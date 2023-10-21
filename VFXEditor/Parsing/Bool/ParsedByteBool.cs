namespace VfxEditor.Parsing {
    public class ParsedByteBool : ParsedBool {
        public ParsedByteBool( string name, bool value ) : base( name, value, 1 ) { }

        public ParsedByteBool( string name ) : base( name, 1 ) { }
    }
}
