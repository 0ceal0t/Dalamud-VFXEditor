using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxRadians : AvfxLiteral<ParsedRadians, float> {
        public AvfxRadians( string name, string avfxName, float value ) : base( avfxName, new( name, value ) ) { }

        public AvfxRadians( string name, string avfxName ) : base( avfxName, new( name ) ) { }
    }
}
