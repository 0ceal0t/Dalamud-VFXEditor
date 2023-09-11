using Dalamud.Logging;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdOffsetsHeader {
        public short SoundCount;
        public short TrackCount;
        public short AudioCount;
        public short UnkOffset;
        public int TrackOffset;
        public int AudioOffset;
        public int LayoutOffset;
        public int RoutingOffset;
        public int AttributeOffset;
        public int EofPaddingSize; // 0
        // padded to 0x20

        public long SoundOffset;
        public readonly List<int> SoundOffsets = new();
        public readonly List<int> TrackOffsets = new();
        public readonly List<int> AudioOffsets = new();
        public readonly List<int> LayoutOffsets = new();
        public readonly List<int> AttributeOffsets = new();

        public readonly bool Modded = false;

        public ScdOffsetsHeader( BinaryReader reader ) {
            SoundCount = reader.ReadInt16();
            TrackCount = reader.ReadInt16();
            AudioCount = reader.ReadInt16();
            UnkOffset = reader.ReadInt16();
            TrackOffset = reader.ReadInt32();
            AudioOffset = reader.ReadInt32();
            LayoutOffset = reader.ReadInt32();
            RoutingOffset = reader.ReadInt32();
            AttributeOffset = reader.ReadInt32();
            EofPaddingSize = reader.ReadInt32();

            var firstOffset = -1;

            SoundOffset = reader.BaseStream.Position;
            ReadOffsets( SoundOffsets, reader, SoundCount, ref firstOffset );
            ReadOffsets( TrackOffsets, reader, TrackCount, ref firstOffset );
            ReadOffsets( AudioOffsets, reader, AudioCount, ref firstOffset );

            if( LayoutOffset != 0 ) {
                ReadOffsets( LayoutOffsets, reader, SoundCount, ref firstOffset );
            }

            if( AttributeOffset != 0 ) {
                var attributeCount = ( firstOffset - AttributeOffset ) / 4;
                ReadOffsets( AttributeOffsets, reader, attributeCount, ref firstOffset );
            }

            if( reader.BaseStream.Position != LayoutOffsets[0] ) {
                PluginLog.Log( $"Actual: {reader.BaseStream.Position:X8} Expected: {LayoutOffsets[0]:X8}" );
                Modded = true;
            }
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( SoundCount );
            writer.Write( TrackCount );
            writer.Write( AudioCount );
            writer.Write( UnkOffset );
            writer.Write( TrackOffset );
            writer.Write( AudioOffset );
            writer.Write( LayoutOffset );
            writer.Write( RoutingOffset );
            writer.Write( AttributeOffset );
            writer.Write( EofPaddingSize );

            WriteOffsets( SoundOffsets, writer );
            WriteOffsets( TrackOffsets, writer );
            WriteOffsets( AudioOffsets, writer );
            WriteOffsets( LayoutOffsets, writer );
            WriteOffsets( AttributeOffsets, writer );
        }

        public static void ReadOffsets( List<int> offsets, BinaryReader reader, int count, ref int firstOffset ) {
            for( var i = 0; i < count; i++ ) {
                var offset = reader.ReadInt32();
                if( offset > 0 && ( firstOffset == -1 || offset < firstOffset ) ) firstOffset = offset;
                offsets.Add( offset );
            }
            FileUtils.PadTo( reader, 16 );
        }

        public static void WriteOffsets( List<int> offsets, BinaryWriter writer ) {
            foreach( var offset in offsets ) writer.Write( offset );
            FileUtils.PadTo( writer, 16 );
        }
    }
}
