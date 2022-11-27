using Dalamud.Logging;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Utils;

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

        public long StartOffset;
        public readonly List<int> StartOffsetList = new();
        public readonly List<int> OffsetList1 = new();
        public readonly List<int> OffsetListSound = new();
        public readonly List<int> OffsetList2 = new();
        public readonly List<int> OffsetList4 = new();

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

            StartOffset = reader.BaseStream.Position;
            ReadOffsets( StartOffsetList, reader, Count1 );
            ReadOffsets( OffsetList1, reader, Count2 );
            ReadOffsets( OffsetListSound, reader, CountSound );
            ReadOffsets( OffsetList2, reader, Count1 );
            // TODO: what if count = 0?
            var countTable4 = ( OffsetList2[0] - Offset4 ) / 4;
            ReadOffsets( OffsetList4, reader, countTable4 );

            //foreach( var a in StartOffsetList ) {
            //    PluginLog.Log( $"start: {a:X8}" );
            //}
            //foreach( var a in OffsetList1 ) {
            //    PluginLog.Log( $"1: {a:X8}" );
            //}

            // o2: first short is the length -- 0x80
            // start: first short like [0A 01] 32 + 4*second byte. In this example 32 + 4*0x01 = 36
            // o1: ??
            // o4: ??

            // [o2] [start] [o1] [o4] [os]

            // padded to 16 between each set of offsets

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
             * --- OFFSETS
             * 
             * [start]
             * - # c1
             * o1
             * - # c2
             * os
             * - # cs
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

            WriteOffsets( StartOffsetList, writer );
            WriteOffsets( OffsetList1, writer );
            WriteOffsets( OffsetListSound, writer );
            WriteOffsets( OffsetList2, writer );
            WriteOffsets( OffsetList4, writer );
        }

        public static void ReadOffsets( List<int> offsets, BinaryReader reader, int count ) {
            // TODO: what if count = 0?
            for( var i = 0; i < count; i++ ) offsets.Add( reader.ReadInt32() );
            FileUtils.PadTo( reader, 16 );
        }

        public static void WriteOffsets( List<int> offsets, BinaryWriter writer ) {
            // TODO: what if count = 0?
            foreach( var offset in offsets ) writer.Write( offset );
            FileUtils.PadTo( writer, 16 );
        }
    }
}
