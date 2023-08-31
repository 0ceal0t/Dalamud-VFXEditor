using System.IO;
using VfxEditor.Parsing.Int;

namespace VfxEditor.SklbFormat.Data {
    public abstract class SklbData {
        public int HavokOffset { get; protected set; }
        public readonly ParsedShort2 Id = new( "Id" );

        public abstract long Write( BinaryWriter writer );

        public abstract void Draw();
    }
}
