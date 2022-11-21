using Dalamud.Logging;
using ImGuiNET;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading;

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

    public class ScdSoundEntry : ScdEntry {
        public int DataLength;
        public int NumChannels;
        public int Frequency;
        public SscfWaveFormat Format;
        public int LoopStart;
        public int LoopEnd;
        public int FirstFrame;
        public short AuxCount;

        public ScdSoundData Data;
        private readonly AudioPlayer Player;

        public ScdSoundEntry( BinaryReader reader ) : base( reader ) {
            Player = new( this );
        }
        public ScdSoundEntry( BinaryReader reader, int offset ) : base( reader, offset ) {
            Player = new( this );
        }

        protected override void Read( BinaryReader reader ) {
            var startOffset = reader.BaseStream.Position;

            DataLength = reader.ReadInt32();
            if( DataLength == 0 ) return;

            NumChannels = reader.ReadInt32();
            Frequency = reader.ReadInt32();
            Format = ( SscfWaveFormat )reader.ReadInt32();
            LoopStart = reader.ReadInt32();
            LoopEnd = reader.ReadInt32();
            FirstFrame = reader.ReadInt32();
            AuxCount = reader.ReadInt16();
            reader.ReadInt16(); // padding

            var chunkStartPos = reader.BaseStream.Position;
            var chunkEndPos = chunkStartPos;
            for( var i = 0; i < AuxCount; i++ ) {
                reader.ReadInt32(); // id
                chunkEndPos += reader.ReadInt32(); // data, skip for now
                reader.BaseStream.Seek( chunkEndPos, SeekOrigin.Begin );
            }

            Data = Format switch {
                SscfWaveFormat.MsAdPcm => new ScdAdpcm( reader, startOffset, this ),
                SscfWaveFormat.Vorbis => new ScdVorbis( reader, chunkEndPos, this ),
                _ => null
            };
        }

        public void Draw( string id ) {
            if( DataLength == 0 ) return;
            Player.Draw( id );
        }

        public void Dispose() => Player.Dispose();
    }
}
