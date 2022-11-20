using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.ScdFormat {
    public class ScdHeader {
        public int Magic;
        public int SectionType;
        public int SedbVersion;
        public byte Endian;
        public byte SscfVersion;
        public short HeaderSize;
        public int FileSize;
        // padded to 0x30 = 48 (28 empty bytes)

        public ScdHeader( BinaryReader reader ) {
            Magic = reader.ReadInt32();
            SectionType = reader.ReadInt32();
            SedbVersion = reader.ReadInt32();
            Endian = reader.ReadByte();
            SscfVersion = reader.ReadByte();
            HeaderSize = reader.ReadInt16();
            FileSize = reader.ReadInt32();
            reader.ReadBytes( 28 );
        }
    }
}
