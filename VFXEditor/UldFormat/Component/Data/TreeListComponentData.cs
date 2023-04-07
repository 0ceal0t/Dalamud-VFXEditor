using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class TreeListComponentData : UldComponentData {
        public TreeListComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedUInt( "Unknown 5" ),
                new ParsedUInt( "Wrap", size: 1),
                new ParsedUInt( "Orientation", size: 1 ),
                new ParsedUInt( "Padding", size: 2 )
            } );
        }
    }
}
