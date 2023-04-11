using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class CheckboxNodeData : UldNodeComponentData {
        public CheckboxNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Text Id" )
            } );
        }
    }
}
