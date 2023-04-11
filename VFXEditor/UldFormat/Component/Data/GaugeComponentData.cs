using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data
{
    public class GaugeComponentData : UldGenericData {
        public GaugeComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedUInt( "Unknown 5" ),
                new ParsedUInt( "Unknown 6" ),
                new ParsedUInt( "Vertical Margin", size: 2 ),
                new ParsedUInt( "Horizontal Margin", size: 2 ),
                new ParsedByteBool( "Is Vertical" ),
                new ParsedReserve( 3 ) // Padding
            } );
        }
    }
}
