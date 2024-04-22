using System.IO;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        private const uint MAGIC = 0x45504842;

        public PhybExtended() { }

        public PhybExtended( BinaryReader reader ) : this() {
            reader.ReadUInt32(); // EPHB
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

        }
    }
}
