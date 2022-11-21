using Dalamud.Logging;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.ScdFormat {
    public class ScdOffsetsHeader {
        public short Count1;
        public short Count2;
        public short CountSound;
        public short UnkOffset;
        public int Offset1;
        public int OffsetSound;
        public int Offset2;
        public int Padding;
        public int Offset4;
        public int Unk2; // 0
        // padded to 0x20

        public int UnkOffsetDiff;

        public ScdOffsetsHeader( BinaryReader reader ) {
            Count1 = reader.ReadInt16();
            Count2 = reader.ReadInt16();
            CountSound = reader.ReadInt16();
            UnkOffset = reader.ReadInt16();
            Offset1 = reader.ReadInt32();
            OffsetSound = reader.ReadInt32();
            Offset2 = reader.ReadInt32();
            Padding = reader.ReadInt32();
            Offset4 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();

            // 4: first short is the length -- 0x80
            // 1: first short like [0A 01] 32 + 4*second byte. In this example 32 + 4*0x01 = 36
            // 2: 0x60 - last 3C bytes don't matter? (probably not)
            // 5: 0x80

            // 4 1 2 5 sound

            /*
             * c1 = 156  624
             * c2 = 107  428
             * cs = 119  476
             * 
             * 80 = pos
             * 704 = o1
             * 1136 = os
             * 1616 = o2
             * 2240 = o4
             * 2051 = unk
             * 
             * 
             * pos
             * - c1
             * o1
             * - c2
             * os
             * - cs
             * o2
             * - c1
             * o4
             * - ....
             * ... first of o4
             */

            // offsets lists 0/1/2/3/4
            // offset lists also padded to 16 bytes

            // In the file, table data is in order: 3, 0, 1, 4, 2
            // padded to 16 bytes between each table data (such as between 3/0)
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( Count1 );
            writer.Write( Count2 );
            writer.Write( CountSound );
            writer.Write( UnkOffset );
            writer.Write( Offset1 );
            writer.Write( OffsetSound );
            writer.Write( Offset2 );
            writer.Write( Padding );
            writer.Write( Offset4 );
            writer.Write( Unk2 );
        }
    }
}
