using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class SliderNodeData : UldNodeComponentData {
        public SliderNodeData() : base() {
            Parsed.AddRange( [
                new ParsedInt( "Min" ),
                new ParsedInt( "Max" ),
                new ParsedInt( "Step" ),
            ] );
        }
    }
}