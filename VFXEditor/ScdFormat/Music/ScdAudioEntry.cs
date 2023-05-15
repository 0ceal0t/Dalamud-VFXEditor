using System;
using System.IO;
using VfxEditor.ScdFormat.Music.Data;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public enum SscfWaveFormat : int {
        Empty = -1,
        Pcm = 0x01,
        Atrac3 = 0x05,
        Vorbis = 0x06,
        Xma = 0x0B,
        MsAdPcm = 0x0C,
        Atrac3Too = 0x0D
    }

    public class ScdAudioEntry : ScdEntry, IUiItem {
        public int DataLength;
        public int NumChannels;
        public int SampleRate;
        public SscfWaveFormat Format;
        public int LoopStart;
        public int LoopEnd;
        public int FirstFrame;
        public short AuxCount;
        public short BitsPerSample;

        public byte[] AuxChunkData;

        public ScdAudioData Data;
        public readonly AudioPlayer Player;

        public bool NoLoop => LoopStart == 0 && LoopEnd == 0;

        public ScdAudioEntry() {
            Player = new( this );
        }

        public override void Read( BinaryReader reader ) {
            DataLength = reader.ReadInt32();
            NumChannels = reader.ReadInt32();
            SampleRate = reader.ReadInt32();
            Format = ( SscfWaveFormat )reader.ReadInt32();
            LoopStart = reader.ReadInt32();
            LoopEnd = reader.ReadInt32();
            FirstFrame = reader.ReadInt32();
            AuxCount = reader.ReadInt16();
            BitsPerSample = reader.ReadInt16();

            if( DataLength == 0 ) {
                AuxChunkData = Array.Empty<byte>();
                return;
            }

            var chunkStartPos = reader.BaseStream.Position;
            var chunkEndPos = chunkStartPos;
            for( var i = 0; i < AuxCount; i++ ) {
                reader.ReadInt32(); // id
                chunkEndPos += reader.ReadInt32(); // data, skip for now
                reader.BaseStream.Seek( chunkEndPos, SeekOrigin.Begin );
            }
            AuxChunkData = GetDataRange( chunkStartPos, chunkEndPos, reader );

            Data = Format switch {
                SscfWaveFormat.MsAdPcm => new ScdAdpcm( reader, this ),
                SscfWaveFormat.Vorbis => new ScdVorbis( reader, this ),
                _ => null
            };
        }

        public void Draw() {
            if( DataLength == 0 ) return;
            Player.Draw();
        }

        public void Dispose() => Player.Dispose();

        public override void Write( BinaryWriter writer ) => Write( writer, out var _ );

        public void Write( BinaryWriter writer, out long padding ) {
            writer.Write( DataLength );
            writer.Write( NumChannels );
            writer.Write( SampleRate );
            writer.Write( ( int )Format );
            writer.Write( LoopStart );
            writer.Write( LoopEnd );
            writer.Write( FirstFrame );
            writer.Write( AuxCount );
            writer.Write( BitsPerSample );

            writer.Write( AuxChunkData );
            Data?.Write( writer );

            padding = FileUtils.PadTo( writer, 16 );
        }

        public static byte[] GetDataRange( long start, long end, BinaryReader reader ) {
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( start, SeekOrigin.Begin );
            var ret = reader.ReadBytes( ( int )( end - start ) );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
            return ret;
        }
    }
}
