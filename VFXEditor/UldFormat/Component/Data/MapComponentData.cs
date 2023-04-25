using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class MapComponentData : UldGenericData {
        public MapComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedUInt( "Unknown 5" ),
                new ParsedUInt( "Unknown 6" ),
                new ParsedUInt( "Unknown 7" ),
                new ParsedUInt( "Unknown 8" ),
                new ParsedUInt( "Unknown 9" ),
                new ParsedUInt( "Unknown 10"),
            } );
        }
    }
}
