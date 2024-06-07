using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedRadians4 : ParsedFloat4 {
        public ParsedRadians4( string name ) : base( name ) { }

        public ParsedRadians4( string name, Vector4 value ) : base( name, value ) { }

        protected override void DrawBody() {
            if( UiUtils.DrawRadians4( Name, Value, out var value ) ) Update( value );
        }
    }
}
