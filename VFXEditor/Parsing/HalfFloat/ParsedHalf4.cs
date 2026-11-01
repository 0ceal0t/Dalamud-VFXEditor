using System.Numerics;

namespace VfxEditor.Parsing {
    public class ParsedHalf4 : ParsedFloat4 {
        protected override bool Half => true;

        public ParsedHalf4( string name, Vector4 value ) : base( name, value ) { }

        public ParsedHalf4( string name ) : base( name ) { }
    }
}
