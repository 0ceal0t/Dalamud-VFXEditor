using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class JournalComponentData : UldGenericData {
        public JournalComponentData() {
            AddUnknown( 32, "Unknown Node Id" );

            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Item Margin", size: 2 ),
                new ParsedUInt( "Basic Margin", size: 2 ),
                new ParsedUInt( "Unknown Margin", size: 2 ),
                new ParsedReserve( 2 ) // Padding
            } );
        }
    }
}
