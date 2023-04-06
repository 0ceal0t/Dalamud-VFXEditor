using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Component.Data {
    public class TextInputComponentData : UldComponentData {
        public TextInputComponentData() {
            for( var i = 1; i <= 16; i++ ) Parsed.Add( new ParsedUInt( $"Unknown {i}" ) );

            Parsed.AddRange( new ParsedBase[] {
                new ParsedIntColor( "Color" ),
                new ParsedIntColor( "IME Color" ),
            } );
        }
    }
}
