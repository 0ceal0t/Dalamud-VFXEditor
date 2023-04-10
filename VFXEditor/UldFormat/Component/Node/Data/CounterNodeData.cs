using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data {
    public class CounterNodeData : UldNodeData {
        public CounterNodeData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Part List Id", size: 2 ),
                new ParsedUInt( "Unknown 1", size: 2 ),
                new ParsedInt( "Part Id", size: 1 ),
                new ParsedInt( "Number Width", size: 1 ),
                new ParsedInt( "Comma Width", size: 1 ),
                new ParsedInt( "Space Width", size: 1 ),
                new ParsedUInt( "Alignment", size: 2 ),
                new ParsedUInt( "Unknown 2", size: 2 ),
            } );
        }
    }
}
