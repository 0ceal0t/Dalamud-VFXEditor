using System;
using System.IO;
using System.Numerics;

namespace VfxEditor.Parsing.HalfFloat {
    public class ParsedHalf4 : ParsedFloat4 {
        public ParsedHalf4( string name, Vector4 value ) : base( name, value ) { }

        public ParsedHalf4( string name ) : base( name ) { }

        public override void Read( BinaryReader reader, int _ ) {
            Value.X = ( float )reader.ReadHalf();
            Value.Y = ( float )reader.ReadHalf();
            Value.Z = ( float )reader.ReadHalf();
            Value.W = ( float )reader.ReadHalf();
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( ( Half )Value.X );
            writer.Write( ( Half )Value.Y );
            writer.Write( ( Half )Value.Z );
            writer.Write( ( Half )Value.W );
        }
    }
}
