using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Node.Data.Component {
    public class CheckboxNodeData : UldNodeComponentData {
        public CheckboxNodeData() : base() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Id" )
            ] );
        }
    }
}