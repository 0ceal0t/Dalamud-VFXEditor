using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class ListNodeData : UldNodeComponentData {
        public ListNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Rows", size: 2 ),
                new ParsedUInt( "Columns", size: 2 ),
            ] );
        }
    }
}
