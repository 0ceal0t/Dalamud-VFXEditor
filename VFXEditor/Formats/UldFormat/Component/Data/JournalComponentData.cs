using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class JournalComponentData : UldGenericData {
        public JournalComponentData() {
            AddUnknown( 38, "Unknown Node Id" ); //added six for 7.2, TBD if this varies

            Parsed.AddRange( [
                new ParsedUInt( "Item Margin", size: 2 ),
                new ParsedUInt( "Basic Margin", size: 2 ),
                new ParsedUInt( "Unknown Margin", size: 2 ),
                new ParsedReserve( 2 ) // Padding
            ] );
        }
    }
}
