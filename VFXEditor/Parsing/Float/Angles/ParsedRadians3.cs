using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedRadians3 : ParsedFloat3 {
        public ParsedRadians3( string name ) : base( name ) { }

        public ParsedRadians3( string name, Vector3 value ) : base( name, value ) { }

        protected override void DrawBody( CommandManager manager ) {
            if( UiUtils.DrawRadians3( Name, Value, out var value ) ) {
                SetValue( manager, value );
            }
        }
    }
}
