using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class WindowNodeData : UldNodeComponentData {
        public WindowNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Title Text Id" ),
                new ParsedUInt( "Subtitle Text Id" ),
                new ParsedByteBool( "Close Button" ),
                new ParsedByteBool( "Settings Button" ),
                new ParsedByteBool( "Help Button" ),
                new ParsedByteBool( "Header" ),
            } );
        }
    }
}
