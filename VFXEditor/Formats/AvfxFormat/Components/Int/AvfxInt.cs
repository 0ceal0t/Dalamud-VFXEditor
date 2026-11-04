using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.AvfxFormat.Components.Int {
    public class AvfxInt : AvfxLiteral<ParsedInt, int> {
        public AvfxInt( string name, string avfxName, int value, int size = 4 ) : base( avfxName, new( name, value, size ) ) { }

        public AvfxInt( string name, string avfxName, int size = 4 ) : base( avfxName, new( name, size: size ) ) { }
    }
}