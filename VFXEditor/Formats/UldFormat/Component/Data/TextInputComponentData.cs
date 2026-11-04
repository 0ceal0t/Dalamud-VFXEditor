using VfxEditor.Parsing;
using VfxEditor.Parsing.Color;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.UldFormat.Component.Data {
    public class TextInputComponentData : UldGenericData {
        public TextInputComponentData() {
            AddUnknown( 16, "Unknown Node Id" );

            Parsed.AddRange( [
                new ParsedSheetColor( "Color" ),
                new ParsedSheetColor( "IME Color" ),
                new ParsedUInt( "Unknown" ),
            ] );
        }
    }
}