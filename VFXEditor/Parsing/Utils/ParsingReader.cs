using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Utils {
    public class ParsingReader {
        public readonly BinaryReader Reader;

        public ParsingReader( BinaryReader reader ) {
            Reader = reader;
        }

        public int ReadInt32() => Reader.ReadInt32();
        public short ReadInt16() => Reader.ReadInt16();
        public byte ReadByte() => Reader.ReadByte();
        public float ReadSingle() => Reader.ReadSingle();
        public string ReadString( int size ) => FileUtils.ReadString( Reader, size );
    }
}
