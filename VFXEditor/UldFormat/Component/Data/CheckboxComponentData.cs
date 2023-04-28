using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class CheckboxComponentData : UldGenericData {
        public CheckboxComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
            } );
        }
    }
}
