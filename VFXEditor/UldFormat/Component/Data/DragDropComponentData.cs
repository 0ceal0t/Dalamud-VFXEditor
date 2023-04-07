using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class DragDropComponentData : UldComponentData {
        public DragDropComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
            } );
        }
    }
}
