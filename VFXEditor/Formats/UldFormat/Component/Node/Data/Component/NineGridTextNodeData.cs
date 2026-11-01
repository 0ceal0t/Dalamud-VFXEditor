using VfxEditor.Parsing;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class NineGridTextNodeData : UldNodeComponentData {
        public NineGridTextNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id" )
            ] );
        }
    }
}
