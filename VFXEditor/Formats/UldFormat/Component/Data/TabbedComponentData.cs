using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class TabbedComponentData : UldGenericData {
        public TabbedComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
            } );
        }
    }
}
