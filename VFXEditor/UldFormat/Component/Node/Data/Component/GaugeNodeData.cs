using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class GaugeNodeData : UldNodeComponentData {
        public GaugeNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedInt( "Indicator" ),
                new ParsedInt( "Min" ),
                new ParsedInt( "Max" ),
                new ParsedInt( "Value" ),
            } );
        }
    }
}
