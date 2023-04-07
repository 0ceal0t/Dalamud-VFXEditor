using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class TextInputComponentData : UldComponentData {
        public TextInputComponentData() {
            AddUnknown( 16 );

            Parsed.AddRange( new ParsedBase[] {
                new ParsedIntColor( "Color" ),
                new ParsedIntColor( "IME Color" ),
            } );
        }
    }
}
