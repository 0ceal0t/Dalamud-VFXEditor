using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class SliderComponentData : UldComponentData {
        public SliderComponentData() {
            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Unknown 1" ),
                new ParsedUInt( "Unknown 2" ),
                new ParsedUInt( "Unknown 3" ),
                new ParsedUInt( "Unknown 4" ),
                new ParsedBool( "Is Vertical", size: 1 ),
                new ParsedUInt( "Left Offset", size: 1 ),
                new ParsedUInt( "Right Offset", size: 1),
                new ParsedInt( "Padding", size: 1)
            } );
        }
    }
}
