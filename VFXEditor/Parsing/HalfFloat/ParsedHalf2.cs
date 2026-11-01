using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedHalf2 : ParsedFloat2 {
        protected override bool Half => true;

        public ParsedHalf2( string name, Vector2 value ) : base( name, value ) { }

        public ParsedHalf2( string name ) : base( name ) { }
    }
}
