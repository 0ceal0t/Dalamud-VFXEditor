using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class ButtonNodeData : UldNodeComponentData {
        public ButtonNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id" )
            ] );
        }
    }
}
