using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.ScdFormat {
    public class ScdOffsetsHeader {
        public short CountTable1;
        public short CountTable2;
        public short CountSound;
        public short Unk1;
        public int OffsetTable1;
        public int OffsetSound;
        public int OffsetTable2;
        public int Padding;
        public int UnkOffset;
        public int Unk2;
        // padded to 0x20

        public ScdOffsetsHeader( BinaryReader reader ) {
            CountTable1 = reader.ReadInt16();
            CountTable2 = reader.ReadInt16();
            CountSound = reader.ReadInt16();
            Unk1 = reader.ReadInt16();
            OffsetTable1 = reader.ReadInt32();
            OffsetSound = reader.ReadInt32();
            OffsetTable2 = reader.ReadInt32();
            Padding = reader.ReadInt32();
            UnkOffset = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
        }
    }
}
