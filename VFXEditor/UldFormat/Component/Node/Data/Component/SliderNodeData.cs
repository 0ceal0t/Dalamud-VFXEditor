using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class SliderNodeData : UldNodeComponentData {
        public SliderNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedInt( "Min" ),
                new ParsedInt( "Max" ),
                new ParsedInt( "Step" ),
            } );
        }
    }
}
