using System.Numerics;
using VfxEditor.Parsing.Float;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Angles {
    public class ParsedRadians3 : ParsedFloat3 {
        public ParsedRadians3( string name ) : base( name ) { }

        public ParsedRadians3( string name, Vector3 value ) : base( name, value ) { }

        protected override void DrawBody() {
            if( UiUtils.DrawRadians3( Name, Value, out var value ) ) Update( value );
        }
    }
}