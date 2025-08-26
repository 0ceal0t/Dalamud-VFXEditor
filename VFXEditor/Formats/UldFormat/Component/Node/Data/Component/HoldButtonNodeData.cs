using VfxEditor.Parsing;
namespace VfxEditor.UldFormat.Component.Node.Data {
    public class HoldButtonNodeData : UldGenericData {
        public HoldButtonNodeData() {
            Parsed.AddRange( [
                new ParsedInt( "Unknown 1", size: 1 ),
                new ParsedInt( "Unknown 2", size: 1 ),
                new ParsedInt( "Unknown 3", size: 1 ),
                new ParsedInt( "Unknown 4", size: 1 ),
                new ParsedInt( "Unknown 5", size: 2 ),
                new ParsedInt( "Unknown 6", size: 2 ),
                new ParsedInt( "Unknown 7" ),
                new ParsedUInt( "Unknown 8", size: 1 ),
                new ParsedUInt( "Unknown 9", size: 1 ),
                new ParsedInt( "Unknown 10", size: 2 ),
            ] );
        }
    }
}