using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Node.Data.Component {
    public class NumericInputNodeData : UldNodeComponentData {
        public NumericInputNodeData() : base() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedInt( "Value" ),
                new ParsedInt( "Max" ),
                new ParsedInt( "Min" ),
                new ParsedInt( "Add" ),
                new ParsedUInt( "Unknown 1" ),
                new ParsedByteBool( "Comma" ),
                new ParsedReserve( 3 ),
            } );
        }
    }
}
