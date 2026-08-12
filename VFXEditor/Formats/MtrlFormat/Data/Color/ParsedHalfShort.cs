using System;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    // Color table rows are stored as arrays of Half; small integer fields (shader/sphere index)
    // are still encoded as a Half whose value equals the integer, not a raw int16.
    public class ParsedHalfShort : ParsedShort {
        public ParsedHalfShort( string name, int value ) : base( name, value ) { }

        public ParsedHalfShort( string name ) : base( name ) { }

        public override void Read( BinaryReader reader ) => Value = ( int )( float )reader.ReadHalf();

        public override void Write( BinaryWriter writer ) => writer.Write( ( Half )Value );
    }
}
