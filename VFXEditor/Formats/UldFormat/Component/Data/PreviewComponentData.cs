using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class PreviewComponentData : UldGenericData {
        public PreviewComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
            } );
        }
    }
}
