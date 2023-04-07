using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class ScrollBarComponentData : UldComponentData {
        public ScrollBarComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedUInt( "Margin", size: 2 ),
                new ParsedBool( "Is Vertical", size: 1 ),
                new ParsedInt( "Padding", size: 1 ),
            } );
        }
    }
}
