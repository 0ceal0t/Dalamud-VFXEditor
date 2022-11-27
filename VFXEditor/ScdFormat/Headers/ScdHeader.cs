using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.ScdFormat {
    public class ScdHeader {
        public int Magic;
        public int SectionType;
        public int SedbVersion;
        public byte Endian;
        public byte AlignmentBits;
        public short HeaderSize; // always 0x30
        public int FileSize; // the ENTIRE file, including the header
        public byte[] UnkPadding;
        // padded to 0x30 = 48 (28 empty bytes)

        public static long FileSizeOffset => 0x10; // where to write the file size

        public ScdHeader( BinaryReader reader ) {
            Magic = reader.ReadInt32();
            SectionType = reader.ReadInt32();
            SedbVersion = reader.ReadInt32();
            Endian = reader.ReadByte();
            AlignmentBits = reader.ReadByte();
            HeaderSize = reader.ReadInt16();
            FileSize = reader.ReadInt32();
            UnkPadding = reader.ReadBytes( 28 );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Magic );
            writer.Write( SectionType );
            writer.Write( SedbVersion );
            writer.Write( Endian );
            writer.Write( AlignmentBits );
            writer.Write( HeaderSize );
            writer.Write( FileSize ); // placeholder
            writer.Write( UnkPadding );
        }

        public static void UpdateFileSize( BinaryWriter writer, long subtract ) {
            var totalSize = writer.BaseStream.Length;
            writer.BaseStream.Seek( FileSizeOffset, SeekOrigin.Begin );
            writer.Write( ( int )( totalSize - subtract ) );
        }
    }
}
