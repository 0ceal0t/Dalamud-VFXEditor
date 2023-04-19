namespace VfxEditor.Parsing {
    public class ParsedByteBool : ParsedBool {
        public ParsedByteBool( string name, bool defaultValue ) : base( name, defaultValue: defaultValue, size: 1 ) { }
        public ParsedByteBool( string name ) : base( name, size: 1 ) { }
    }
}
