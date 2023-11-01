using System;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing.HalfFloat {
    public class ParsedHalf3 : ParsedFloat3 {
        public ParsedHalf3( string name, Vector3 value ) : base( name, value ) { }

        public ParsedHalf3( string name ) : base( name ) { }

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = ( float )reader.ReadHalf();
            Value.Y = ( float )reader.ReadHalf();
            Value.Z = ( float )reader.ReadHalf();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( Half )Value.X );
            writer.Write( ( Half )Value.Y );
            writer.Write( ( Half )Value.Z );
        }
    }
}
