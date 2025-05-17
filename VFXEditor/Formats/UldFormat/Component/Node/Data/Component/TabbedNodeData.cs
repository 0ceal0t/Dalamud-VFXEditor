using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class TabbedNodeData : UldNodeComponentData {
        public TabbedNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id", size: 2 ),
                new ParsedUInt( "Group Id", size: 2 ),
                new ParsedUInt( "Unknown 1" ),
            ] );
        }
    }
}
