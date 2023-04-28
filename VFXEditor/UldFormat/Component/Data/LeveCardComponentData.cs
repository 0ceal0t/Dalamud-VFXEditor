using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class LeveCardComponentData : UldGenericData {
        public LeveCardComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
            } );
        }
    }
}
