using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class IconWithTextComponentData : UldGenericData {
        public IconWithTextComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
            } );
        }
    }
}
