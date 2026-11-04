using System.Numerics;
using VfxEditor.Parsing.Float;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Angles {
    public class ParsedDegrees3 : ParsedFloat3 {
        public ParsedDegrees3( string name ) : base( name ) { }

        public ParsedDegrees3( string name, Vector3 value ) : base( name, value ) { }

        protected override void DrawBody() {
            if( UiUtils.DrawDegrees3( Name, Value, out var newValue ) ) Update( newValue );
        }
    }
}