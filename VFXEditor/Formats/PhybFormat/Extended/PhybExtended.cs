using System.IO;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        public const uint MAGIC_PACK = 0x4B434150;
        public const uint MAGIC_EPHD = 0x42485045;

        private readonly byte[] EphdData;
        private readonly byte[] PackData;

        public int Size => EphdData.Length + 0x10 + PackData.Length;

        public PhybExtended() { }

        public PhybExtended( BinaryReader reader ) : this() {
            var ephdCount = 0;
            for( var pos = reader.BaseStream.Length - 0x18; pos >= 0; pos-- ) { // go backwards from the end
                reader.BaseStream.Position = pos;
                if( reader.ReadUInt32() == MAGIC_EPHD ) {
                    ephdCount++;
                    if( ephdCount == 2 ) { // found it. kinda jank but it works
                        reader.ReadUInt32(); // 1
                        var size = reader.ReadUInt32();
                        reader.ReadUInt32(); // 0
                        EphdData = reader.ReadBytes( ( int )size );
                        PackData = reader.ReadBytes( 0x18 );
                        return;
                    }
                }
            }

            // int
            // int (size of stuff after)
            // int (padding?)
            // int (usually 14?)
            // EPHB
            // ...
            // starts with 00 00 0A 00 08 00 00 00 00 00 04 00 0A 00 00 00 04 00 00 00
            // ^ bunch of offsets
            // count
            // offsets * count
            // ^ offsets are relative to themselves, go in reverse order

            // [UNK DATA?]

            // Data
            // - negative offset to something
            // 

            // PACK (always 0x18 bytes)
            // short
            // short (EPHB count, only seen 1 so far)
            // offset from [PACK] to first EPHB (signed int, can be negative)
            // short
            // short
            // short
            // length of first EPHB to end of pack
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( MAGIC_EPHD );
            writer.Write( 1 );
            writer.Write( EphdData.Length );
            writer.Write( 0 );
            writer.Write( EphdData );
            writer.Write( PackData );
        }
    }
}
