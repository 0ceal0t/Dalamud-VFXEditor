using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class ButtonComponentData : UldGenericData {
        public ButtonComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Node Id" ),
                new ParsedUInt( "Background Node Id" ),
            ] );
        }
    }
}
