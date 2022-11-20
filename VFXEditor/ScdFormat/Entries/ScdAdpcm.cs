using NAudio.Wave;
using System.IO;

namespace VfxEditor.ScdFormat {
    public class ScdAdpcm : ScdSoundData {
        public byte[] Data;
        public WaveFormat Format;

        public ScdAdpcm( BinaryReader reader, long startOffset, ScdSoundEntry entry ) {
            Format = WaveFormat.FromFormatChunk( reader, entry.FirstFrame );

            reader.BaseStream.Seek( startOffset + entry.FirstFrame + 0x20, SeekOrigin.Begin );
            Data = reader.ReadBytes( entry.DataLength );
        }

        public override WaveStream GetStream() {
            var ms = new MemoryStream( Data, 0, Data.Length, false );
            return new RawSourceWaveStream( ms, Format );
        }
    }
}
