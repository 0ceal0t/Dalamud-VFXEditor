using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data
{
    public class TextInputComponentData : UldGenericData {
        public TextInputComponentData() {
            AddUnknown( 16 );

            Parsed.AddRange( new ParsedBase[] {
                new ParsedUInt( "Color" ),
                new ParsedUInt( "IME Color" ),
            } );
        }
    }
}
