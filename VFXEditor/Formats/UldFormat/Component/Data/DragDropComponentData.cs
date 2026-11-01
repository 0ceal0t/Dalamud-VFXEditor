using VfxEditor.Parsing;

namespace VfxEditor.Formats.UldFormat.Component.Data {
    public class DragDropComponentData : UldGenericData {
        public DragDropComponentData() {
            Parsed.AddRange( [
                new ParsedUInt( "Unknown Node Id 1" ),
            ] );
        }
    }
}
