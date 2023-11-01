using System;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing.HalfFloat {
    public class ParsedHalf2 : ParsedFloat2 {
        public ParsedHalf2( string name, Vector2 value ) : base( name, value ) { }

        public ParsedHalf2( string name ) : base( name ) { }

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = ( float )reader.ReadHalf();
            Value.Y = ( float )reader.ReadHalf();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( Half )Value.X );
            writer.Write( ( Half )Value.Y );
        }
    }
}
