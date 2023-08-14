using System.IO;
using VfxEditor.Parsing.Int;

namespace VfxEditor.SklbFormat.Data {
    public abstract class SklbData {
        public int HavokOffset { get; protected set; }
        public readonly ParsedShort2 Id = new( "Id" );
        public readonly ParsedShort2 Parent1 = new( "Parent 1" );
        public readonly ParsedShort2 Parent2 = new( "Parent 2" );
        public readonly ParsedShort2 Parent3 = new( "Parent 3" );
        public readonly ParsedShort2 Parent4 = new( "Parent 4" );

        public abstract long Write( BinaryWriter writer );

        public abstract void Draw();
    }
}
