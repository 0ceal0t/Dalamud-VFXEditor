using HelixToolkit.SharpDX.Core.Helper;
using Lumina.Extensions;
using NAudio.Vorbis;
using NAudio.Wave;
using System.IO;

namespace VfxEditor.ScdFormat {
    public class ScdVorbis : ScdSoundData {
        public short EncodeMode;
        public short EncodeByte;
        public int SeekTableSize;
        public int VorbisHeaderSize;

        public byte[] Data;
        public byte[] RawData;

        // TODO: download as OGG?

        public ScdVorbis( BinaryReader reader, long chunkEndPos, ScdSoundEntry entry ) {
            var startPos = reader.BaseStream.Position;

            EncodeMode = reader.ReadInt16();
            EncodeByte = reader.ReadInt16();
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            SeekTableSize = reader.ReadInt32();
            VorbisHeaderSize = reader.ReadInt32();
            reader.ReadInt32();
            //1c6c
            //Vorbis Header + Data
            reader.BaseStream.Seek( chunkEndPos + 0x20 + SeekTableSize, SeekOrigin.Begin );

            var vorbisHeader = reader.ReadBytes( VorbisHeaderSize );

            if( EncodeMode == 0x2002 && EncodeByte != 0x00 ) ScdUtils.XorDecode( vorbisHeader, ( byte )EncodeByte );

            var oggData = reader.ReadBytes( entry.DataLength );

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter( ms );
            bw.Write( vorbisHeader );
            bw.Write( oggData );

            Data = ms.ToArray();

            if( EncodeMode == 0x2003 ) ScdUtils.XorDecodeFromTable( Data, oggData.Length );

            var endPos = reader.BaseStream.Position;
            RawData = ScdSoundEntry.GetDataRange( startPos, endPos, reader );
        }

        public override WaveStream GetStream() {
            var ms = new MemoryStream( Data, 0, Data.Length, false );
            return new VorbisWaveReader( ms );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( RawData );
        }
    }
}
