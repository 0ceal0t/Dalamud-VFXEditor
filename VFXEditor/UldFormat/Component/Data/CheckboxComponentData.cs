using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class CheckboxComponentData : UldGenericData {
        public CheckboxComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
            } );
        }
    }
}
