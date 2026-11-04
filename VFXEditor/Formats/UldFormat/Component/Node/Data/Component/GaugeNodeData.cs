using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class GaugeNodeData : UldNodeComponentData {
        public GaugeNodeData() : base() {
            Parsed.AddRange( [
                new ParsedInt( "Indicator" ),
                new ParsedInt( "Min" ),
                new ParsedInt( "Max" ),
                new ParsedInt( "Value" ),
            ] );
        }
    }
}