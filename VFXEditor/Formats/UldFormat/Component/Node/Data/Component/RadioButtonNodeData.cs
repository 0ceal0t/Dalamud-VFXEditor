using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class RadioButtonNodeData : UldNodeComponentData {
        public RadioButtonNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id" ),
                new ParsedUInt( "Group Id" ),
            ] );
        }
    }
}