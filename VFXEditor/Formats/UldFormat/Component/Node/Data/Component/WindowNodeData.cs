using VfxEditor.Parsing;
using VfxEditor.Parsing.Bool;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class WindowNodeData : UldNodeComponentData {
        public WindowNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Title Text Id" ),
                new ParsedUInt( "Subtitle Text Id" ),
                new ParsedByteBool( "Close Button" ),
                new ParsedByteBool( "Settings Button" ),
                new ParsedByteBool( "Help Button" ),
                new ParsedByteBool( "Header" ),
            ] );
        }
    }
}