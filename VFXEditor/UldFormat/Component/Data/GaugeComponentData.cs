using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class GaugeComponentData : UldGenericData {
        public GaugeComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedUInt( "Unknown Node Id 5" ),
                new ParsedUInt( "Unknown Node Id 6" ),
                new ParsedUInt( "Vertical Margin", size: 2 ),
                new ParsedUInt( "Horizontal Margin", size: 2 ),
                new ParsedByteBool( "Is Vertical" ),
                new ParsedReserve( 3 ) // Padding
            } );
        }
    }
}
