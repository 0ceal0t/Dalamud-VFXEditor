using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class TreeListComponentData : UldGenericData {
        public TreeListComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedUInt( "Unknown Node Id 5" ),
                new ParsedUInt( "Wrap", size: 1),
                new ParsedUInt( "Orientation", size: 1 ),
                new ParsedUInt( "Padding", size: 2 )
            } );
        }
    }
}
