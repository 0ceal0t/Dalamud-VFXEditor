using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class ButtonNodeData : UldNodeComponentData {
        public ButtonNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id" )
            ] );
        }
    }
}