using VfxEditor.Parsing;

namespace VfxEditor.Formats.AvfxFormat.Components.Bool {
    public class AvfxBool : AvfxLiteral<ParsedNullableBool, bool?> {
        public AvfxBool( string name, string avfxName, bool value, int size = 4 ) : base( avfxName, new( name, value, size ) ) { }

        public AvfxBool( string name, string avfxName, int size = 4 ) : base( avfxName, new( name, size ) ) { }
    }
}
