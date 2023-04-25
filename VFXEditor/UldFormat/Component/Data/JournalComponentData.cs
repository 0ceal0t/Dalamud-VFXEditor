using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class JournalComponentData : UldGenericData {
        public JournalComponentData() {
            AddUnknown( 32 );

            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Margin", size: 2 ),
                new ParsedUInt( "Unknown 33", size: 2 ),
                new ParsedUInt( "Unknown 34", size: 2 ),
                new ParsedReserve( 2 ) // Padding
            } );
        }
    }
}
