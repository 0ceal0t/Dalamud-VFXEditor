using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class ScrollBarComponentData : UldGenericData {
        public ScrollBarComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedUInt( "Margin", size: 2 ),
                new ParsedByteBool( "Is Vertical" ),
                new ParsedInt( "Padding", size: 1 ),
            } );
        }
    }
}
