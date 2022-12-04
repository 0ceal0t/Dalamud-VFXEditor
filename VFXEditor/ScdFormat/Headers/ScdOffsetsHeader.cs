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

            SoundOffset = reader.BaseStream.Position;
            ReadOffsets( SoundOffsets, reader, SoundCount );
            ReadOffsets( TrackOffsets, reader, TrackCount );
            ReadOffsets( AudioOffsets, reader, AudioCount );

            if( LayoutOffset != 0 ) {
                ReadOffsets( LayoutOffsets, reader, SoundCount );
            }
            if( AttributeOffset != 0 ) {
                var attributeCount = ( LayoutOffsets[0] - AttributeOffset ) / 4;
                ReadOffsets( AttributeOffsets, reader, attributeCount );
            }

            //PluginLog.Log( $"Layout: {LayoutOffset:X8} Routing: {RoutingOffset:X8} Attribute: {AttributeOffset:X8} cs: {SoundCount}" );
            //PluginLog.Log( $"Sound: {SoundOffset:X8} Track: {TrackOffset:X8} Audio: {AudioOffset:X8} first: {LayoutOffsets[0]:X8}" );

            /*
            if( scdHeader.LayoutOffset != 0 ) {
                Reader.Position = scdHeader.LayoutOffset;
                _layoutOffset = Reader.ReadUInt32();
            }

            if( scdHeader.RoutingOffset != 0 ) {
                Reader.Position = scdHeader.RoutingOffset;
                _routingOffset = Reader.ReadUInt32();
            }

            if( scdHeader.AttributeOffset != 0 ) {
                Reader.Position = scdHeader.AttributeOffset;
                _attributeOffset = Reader.ReadUInt32();
            }
            */
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

        public static void ReadOffsets( List<int> offsets, BinaryReader reader, int count ) {
            for( var i = 0; i < count; i++ ) offsets.Add( reader.ReadInt32() );
            FileUtils.PadTo( reader, 16 );
        }

        public static void WriteOffsets( List<int> offsets, BinaryWriter writer ) {
            foreach( var offset in offsets ) writer.Write( offset );
            FileUtils.PadTo( writer, 16 );
        }
    }
}
