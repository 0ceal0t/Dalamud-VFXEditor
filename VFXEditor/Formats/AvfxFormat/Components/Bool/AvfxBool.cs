using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxBool : AvfxLiteral<ParsedNullableBool, bool?> {
        public AvfxBool( string name, string avfxName, bool value, int size = 4 ) : base( avfxName, new( name, value, size ) ) { }

        public AvfxBool( string name, string avfxName, int size = 4 ) : base( avfxName, new( name, size ) ) { }
    }
}
