using VfxEditor.Parsing;
using VfxEditor.Parsing.Color;

namespace VfxEditor.UldFormat.Component.Data {
    public class NumericInputComponentData : UldGenericData {
        public NumericInputComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedUInt( "Unknown Node Id 5" ),
                new ParsedSheetColor( "Color" ),
            ] );
        }
    }
}
