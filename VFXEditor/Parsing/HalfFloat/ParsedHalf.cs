using System;
using System.IO;

namespace VfxEditor.Parsing.HalfFloat {
    public class ParsedHalf : ParsedFloat {
        public ParsedHalf( string name, float value ) : base( name, value ) { }

        public ParsedHalf( string name ) : base( name ) { }

        public override void Read( BinaryReader reader, int _ ) {
            Value = ( float )reader.ReadHalf();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( ( Half )Value );
    }
}