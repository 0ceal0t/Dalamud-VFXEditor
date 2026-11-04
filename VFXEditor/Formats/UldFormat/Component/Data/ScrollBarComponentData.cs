using VfxEditor.Parsing;
using VfxEditor.Parsing.Bool;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Data {
    public class ScrollBarComponentData : UldGenericData {
        public ScrollBarComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedUInt( "Margin", size: 2 ),
                new ParsedByteBool( "Is Vertical" ),
                new ParsedInt( "Padding", size: 1 ),
            ] );
        }
    }
}