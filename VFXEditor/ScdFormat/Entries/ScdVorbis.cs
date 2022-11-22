using Dalamud.Logging;
using NAudio.Vorbis;
using NAudio.Wave;
using NVorbis;
using System;
using System.IO;

namespace VfxEditor.ScdFormat {
    public class ScdVorbis : ScdSoundData {
        public short EncodeMode;
        public short EncodeByte;
        public int Unk1;
        public int Unk2;
        public float Unk3;
        public int SeekTableSize;
        public int VorbisHeaderSize;
        public int Unk4;
        public int Unk5;

        public byte[] SeekTableData;
        public byte[] VorbisHeaderData;
        public byte[] OggData;

        public byte[] DecodedData;

        // https://github.com/CrimsonOrion/CS-FFXIV-Data-Worker/blob/af126fd74139f54722b4dd8feea87c54077caeb7/FFXIV%20Data%20Worker/OggToScd.cs
        // https://github.com/Soreepeong/XivAlexander/blob/0b1077ebbcd2bf13955169fddc2bc38c218d19fe/XivAlexanderCommon/Sqex/Sound/Writer.cpp#L63

        public ScdVorbis( BinaryReader reader, ScdSoundEntry entry ) {
            EncodeMode = reader.ReadInt16();
            EncodeByte = reader.ReadInt16();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadSingle();
            SeekTableSize = reader.ReadInt32();
            VorbisHeaderSize = reader.ReadInt32();
            Unk4 = reader.ReadInt32();
            Unk5 = reader.ReadInt32();

            //1c6c
            //Vorbis Header + Data
            SeekTableData = reader.ReadBytes( SeekTableSize );
            VorbisHeaderData = reader.ReadBytes( VorbisHeaderSize );

            var decodedHeader = new byte[VorbisHeaderData.Length];
            Buffer.BlockCopy( VorbisHeaderData, 0, decodedHeader, 0, decodedHeader.Length );
            if( EncodeMode == 0x2002 && EncodeByte != 0x00 ) ScdUtils.XorDecode( decodedHeader, ( byte )EncodeByte );

            OggData = reader.ReadBytes( entry.DataLength );

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter( ms );
            bw.Write( decodedHeader );
            bw.Write( OggData );

            DecodedData = ms.ToArray();

            if( EncodeMode == 0x2003 ) ScdUtils.XorDecodeFromTable( DecodedData, OggData.Length );
        }

        public override WaveStream GetStream() {
            var ms = new MemoryStream( DecodedData, 0, DecodedData.Length, false );
            return new VorbisWaveReader( ms );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( EncodeMode );
            writer.Write( EncodeByte );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
            writer.Write( SeekTableSize );
            writer.Write( VorbisHeaderSize );
            writer.Write( Unk4 );
            writer.Write( Unk5 );

            writer.Write( SeekTableData );
            writer.Write( VorbisHeaderData );
            writer.Write( OggData );
        }

        public static void ImportWav( string path, ScdSoundEntry entry ) {
            ScdUtils.ConvertToOgg( path );
            var data = ( ScdVorbis )entry.Data;
            using var waveFile = new VorbisWaveReader( ScdManager.ConvertOgg );
        }
    }
}
