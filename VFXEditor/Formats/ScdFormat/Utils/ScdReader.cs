using System.Collections.Generic;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ScdFormat.Utils {
    public class ScdReader {
        public readonly List<int> SoundOffsets = [];
        public readonly List<int> TrackOffsets = [];
        public readonly List<int> AudioOffsets = [];
        public readonly List<int> LayoutOffsets = [];
        public readonly List<int> AttributeOffsets = [];

        public readonly bool Modded = false;
        public readonly short UnknownOffset;
        public readonly int EofPaddingSize;

        public ScdReader( BinaryReader reader ) {
            var soundCount = reader.ReadInt16();
            var trackCount = reader.ReadInt16();
            var audioCount = reader.ReadInt16();
            UnknownOffset = reader.ReadInt16();
            reader.ReadInt32(); // track offset
            reader.ReadInt32(); // audio offset
            var layoutOffset = reader.ReadInt32();
            var routingOffset = reader.ReadInt32();
            var attributeOffset = reader.ReadInt32();
            EofPaddingSize = reader.ReadInt32();

            var firstOffset = -1;

            ReadOffsets( SoundOffsets, reader, soundCount, ref firstOffset );
            ReadOffsets( TrackOffsets, reader, trackCount, ref firstOffset );
            ReadOffsets( AudioOffsets, reader, audioCount, ref firstOffset );

            if( layoutOffset != 0 ) {
                ReadOffsets( LayoutOffsets, reader, soundCount, ref firstOffset );
            }

            if( attributeOffset != 0 ) {
                var attributeCount = ( firstOffset - attributeOffset ) / 4;
                ReadOffsets( AttributeOffsets, reader, attributeCount, ref firstOffset );
            }

            // ==============================

            if( reader.BaseStream.Position != LayoutOffsets[0] ) {
                Dalamud.Error( $"Actual: {reader.BaseStream.Position:X8} Expected: {LayoutOffsets[0]:X8}" );
                Modded = true;
            }

            if( routingOffset != 0 ) {
                Dalamud.Error( $"Routing: {routingOffset:X4}" );
            }
        }

        public static void ReadOffsets( List<int> offsets, BinaryReader reader, int count, ref int firstOffset ) {
            for( var i = 0; i < count; i++ ) {
                var offset = reader.ReadInt32();
                if( offset > 0 && ( firstOffset == -1 || offset < firstOffset ) ) firstOffset = offset;
                offsets.Add( offset );
            }
            FileUtils.PadTo( reader, 16 );
        }
    }
}
