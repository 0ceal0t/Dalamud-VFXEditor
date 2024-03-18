using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class RadioButtonNodeData : UldNodeComponentData {
        public RadioButtonNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id" ),
                new ParsedUInt( "Group Id" ),
            ] );
        }
    }
}
