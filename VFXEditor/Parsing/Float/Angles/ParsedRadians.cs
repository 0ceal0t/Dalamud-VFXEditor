using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedRadians : ParsedFloat {
        public ParsedRadians( string name ) : base( name ) { }

        public ParsedRadians( string name, float value ) : base( name, value ) { }

        protected override void DrawBody() {
            if( UiUtils.DrawRadians( Name, Value, out var value ) ) {
                SetValue( value );
            }
        }
    }
}
