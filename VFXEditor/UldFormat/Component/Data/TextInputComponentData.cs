using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data
{
    public class TextInputComponentData : UldGenericData {
        public TextInputComponentData() {
            AddUnknown( 16 );

            Parsed.AddRange( new ParsedBase[] {
                new ParsedIntColor( "Color" ),
                new ParsedIntColor( "IME Color" ),
            } );
        }
    }
}
