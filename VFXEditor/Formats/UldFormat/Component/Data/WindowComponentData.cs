using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class WindowComponentData : UldGenericData {
        public WindowComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedUInt( "Unknown Node Id 5" ),
                new ParsedUInt( "Unknown Node Id 6" ),
                new ParsedUInt( "Unknown Node Id 7" ),
                new ParsedUInt( "Unknown Node Id 8" ),
            } );
        }
    }
}
