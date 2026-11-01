using VfxEditor.Parsing;
namespace VfxEditor.Formats.UldFormat.Component.Data {
    public class HoldButtonComponentData : UldGenericData {
        public HoldButtonComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Text Node Id" ),
                new ParsedUInt( "NineGrid Node Id" ),
                new ParsedUInt( "Container Node Id" ),
                new ParsedUInt( "Image Node Id" ),
            ] );
        }
    }
}