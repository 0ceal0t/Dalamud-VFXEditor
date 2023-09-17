using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxFloat : AvfxLiteral<ParsedFloat, float> {
        public AvfxFloat( string name, string avfxName, float value ) : base( avfxName, new( name, value ) ) { }

        public AvfxFloat( string name, string avfxName ) : base( avfxName, new( name ) ) { }
    }
}
