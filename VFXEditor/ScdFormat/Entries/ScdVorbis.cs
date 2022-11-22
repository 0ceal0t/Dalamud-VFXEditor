using Dalamud.Logging;
using NAudio.Vorbis;
using NAudio.Wave;
using SharpDX.Text;
using System;
using System.IO;

namespace VfxEditor.ScdFormat {
    public class ScdVorbis : ScdSoundData {
        private readonly bool Imported = false;

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

            // ==== IMPORTED =====
            var seekTableString = Encoding.ASCII.GetString( BitConverter.GetBytes( SeekTableSize ) );
            if( seekTableString.EndsWith("vor") ) { // "vorbis"
                Imported = true;
                VorbisHeaderData = reader.ReadBytes( 0x35C );
                OggData = reader.ReadBytes( entry.DataLength + 0x10 );
                DecodedData = OggData;
                return;
            }

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

            if( Imported ) {
                writer.Write( VorbisHeaderData );
                writer.Write( OggData );
                return;
            }

            writer.Write( VorbisHeaderSize );
            writer.Write( Unk4 );
            writer.Write( Unk5 );
            writer.Write( SeekTableData );
            writer.Write( VorbisHeaderData );
            writer.Write( OggData );
        }

        // Giga-scuffed
        public static void ImportOgg( string path, ScdSoundEntry entry ) {
            using var waveFile = new VorbisWaveReader( path );

            var oggData = File.ReadAllBytes( path );
            var waveFormat = waveFile.WaveFormat;

            var rawHeader = File.ReadAllBytes( ScdUtils.VorbisHeader );

            using var writerMs = new MemoryStream();
            using var writer = new BinaryWriter( writerMs );
            writer.Write( rawHeader );
            writer.Write( oggData );

            writer.BaseStream.Seek( 0, SeekOrigin.Begin );
            writer.Write( oggData.Length - 0x10 ); // update data length
            writer.Write( waveFormat.Channels );
            writer.Write( waveFormat.SampleRate );

            writer.BaseStream.Seek( 0x10, SeekOrigin.Begin );
            writer.Write( 0 ); // loop start
            writer.Write( oggData.Length ); // loop end

            var newEntryData = writerMs.ToArray();

            using var readerMs = new MemoryStream( newEntryData );
            using var reader = new BinaryReader( readerMs );

            var newEntry = new ScdSoundEntry( reader );

            Plugin.ScdManager.CurrentFile.Replace( entry, newEntry );
            entry.Dispose();
        }

        public static void ImportWav( string path, ScdSoundEntry entry ) {
            ScdUtils.ConvertToOgg( path );
            ImportOgg( ScdManager.ConvertOgg, entry );
        }
    }
}
