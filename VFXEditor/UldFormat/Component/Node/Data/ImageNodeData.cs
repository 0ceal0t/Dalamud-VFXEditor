using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public class ImageNodeData : UldNodeData {
        public ImageNodeData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Part List Id", size: 2 ),
                new ParsedUInt( "Unknown 1", size: 2 ),
                new ParsedUInt( "Part Id" ),
                new ParsedBool( "Flip H" ),
                new ParsedBool( "Flip V" ),
                new ParsedInt( "Wrap", size: 1 ),
                new ParsedInt( "Unknown 2", size: 1 )
            } );
        }
    }
}
