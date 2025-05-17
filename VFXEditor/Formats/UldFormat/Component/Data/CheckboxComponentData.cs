using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class CheckboxComponentData : UldGenericData {
        public CheckboxComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Node Id" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
            ] );
        }
    }
}
