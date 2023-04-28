using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class DropDownComponentData : UldGenericData {
        public DropDownComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
            } );
        }
    }
}
