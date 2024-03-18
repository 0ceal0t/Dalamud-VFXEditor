using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class ListItemComponentData : UldGenericData {
        public ListItemComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
            ] );
        }
    }
}
