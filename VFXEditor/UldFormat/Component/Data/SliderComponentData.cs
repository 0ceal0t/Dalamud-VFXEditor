using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class SliderComponentData : UldGenericData {
        public SliderComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown Node Id 1" ),
                new ParsedUInt( "Unknown Node Id 2" ),
                new ParsedUInt( "Unknown Node Id 3" ),
                new ParsedUInt( "Unknown Node Id 4" ),
                new ParsedByteBool( "Is Vertical" ),
                new ParsedUInt( "Left Offset", size: 1 ),
                new ParsedUInt( "Right Offset", size: 1),
                new ParsedInt( "Padding", size: 1)
            } );
        }
    }
}
