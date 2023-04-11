using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class ListItemNodeData : UldNodeComponentData {
        public ListItemNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedByteBool( "Toggle" ),
                new ParsedReserve( 3 ),
            } );
        }
    }
}
