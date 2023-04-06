using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class NumericInputComponentData : UldComponentData {
        public NumericInputComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedUInt( "Unknown 5" ),
                new ParsedIntColor( "Color" ),
            } );
        }
    }
}
