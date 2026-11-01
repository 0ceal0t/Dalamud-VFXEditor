using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedHalf3 : ParsedFloat3 {
        protected override bool Half => true;

        public ParsedHalf3( string name, Vector3 value ) : base( name, value ) { }

        public ParsedHalf3( string name ) : base( name ) { }
    }
}
