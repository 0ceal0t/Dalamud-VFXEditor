using System.IO;

namespace VfxEditor.Parsing.Utils {
    // Shared variable-width (4/2/1 byte) read/write logic used by ParsedInt, ParsedUInt and ParsedBool
    public static class SizedNumberUtils {
        public static long ReadSigned( BinaryReader reader, int size ) => size switch {
            4 => reader.ReadInt32(),
            2 => reader.ReadInt16(),
            1 => reader.ReadByte(),
            _ => reader.ReadByte()
        };

        public static ulong ReadUnsigned( BinaryReader reader, int size ) => size switch {
            4 => reader.ReadUInt32(),
            2 => reader.ReadUInt16(),
            1 => reader.ReadByte(),
            _ => reader.ReadByte()
        };

        public static void WriteSigned( BinaryWriter writer, long value, int size ) {
            if( size == 4 ) writer.Write( ( int )value );
            else if( size == 2 ) writer.Write( ( short )value );
            else writer.Write( ( byte )value );
        }

        public static void WriteUnsigned( BinaryWriter writer, ulong value, int size ) {
            if( size == 4 ) writer.Write( ( uint )value );
            else if( size == 2 ) writer.Write( ( short )( uint )value );
            else writer.Write( ( byte )value );
        }
    }
}
