using Dalamud.Logging;
using NAudio.Vorbis;
using NAudio.Wave;
using NVorbis;
using SharpDX.Text;
using System;
using System.IO;

namespace VfxEditor.ScdFormat.Music.Data {
    public class ScdVorbis : ScdAudioData {
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

        public ScdVorbis( BinaryReader reader, ScdAudioEntry entry ) {
            EncodeMode = reader.ReadInt16();
            EncodeByte = reader.ReadInt16();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadSingle();
            SeekTableSize = reader.ReadInt32();

            // ==== IMPORTED =====
            var seekTableString = Encoding.ASCII.GetString( BitConverter.GetBytes( SeekTableSize ) );
            if( seekTableString.EndsWith( "vor" ) ) { // "vorbis"
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

        public override int SamplesToBytes( int samples ) {
            var ms = new MemoryStream( DecodedData, 0, DecodedData.Length, false );
            var reader = new VorbisReader( ms );

            var bytes = SamplesToBytesInternal( samples, reader );

            reader.Dispose();
            ms.Dispose();

            return bytes;
        }

        public override int TimeToBytes( float time ) {
            var ms = new MemoryStream( DecodedData, 0, DecodedData.Length, false );
            var reader = new VorbisReader( ms ) {
                TimePosition = TimeSpan.FromSeconds( time )
            };

            var samples = reader.SamplePosition;

            reader.SeekTo( 0 );

            var bytes = SamplesToBytesInternal( samples, reader );

            reader.Dispose();
            ms.Dispose();

            return bytes;
        }

        private static int SamplesToBytesInternal( long samples, VorbisReader reader ) {
            var buffer = new float[reader.Channels];
            reader.StreamStats.ResetStats();

            for( var i = 0; i < samples; i++ ) {
                reader.ReadSamples( buffer, 0, buffer.Length );
            }

            return CurrentBytes( reader );
        }

        public override void BytesToLoopStartEnd( int loopStart, int loopEnd, out double startTime, out double endTime ) {
            if( loopStart == 0 && loopEnd == 0 ) {
                startTime = 0;
                endTime = 0;
                return;
            }

            var ms = new MemoryStream( DecodedData, 0, DecodedData.Length, false );
            var reader = new VorbisReader( ms );

            startTime = 0;
            endTime = reader.TotalTime.TotalSeconds;

            var buffer = new float[reader.Channels];
            reader.StreamStats.ResetStats();

            var prevBytes = 0;
            var startFound = false;
            var endFound = false;

            for( var i = 0; i < reader.TotalSamples; i++ ) {
                reader.ReadSamples( buffer, 0, buffer.Length );

                var currentBytes = CurrentBytes( reader );
                var currentTime = reader.TimePosition.TotalSeconds;

                if( !startFound && prevBytes < loopStart && currentBytes >= loopStart ) {
                    startFound = true;
                    startTime = currentTime;
                }

                if( !endFound && prevBytes < loopEnd && currentBytes >= loopEnd ) {
                    endFound = true;
                    endTime = currentTime;
                }

                if( startFound && endFound ) break;
                prevBytes = currentBytes;
            }

            reader.Dispose();
            ms.Dispose();
        }

        private static int CurrentBytes( VorbisReader reader ) => ( int )( ( reader.StreamStats.AudioBits + reader.StreamStats.ContainerBits + reader.StreamStats.WasteBits + reader.StreamStats.OverheadBits ) / 8 );

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
        public static void ImportOgg( string path, ScdAudioEntry oldEntry ) {
            using var oggReader = new VorbisReader( path );

            var loopStartTag = oggReader.Tags.GetTagSingle( "LoopStart" );
            var loopEndTag = oggReader.Tags.GetTagSingle( "LoopEnd" );

            var oggData = File.ReadAllBytes( path );

            var rawHeader = File.ReadAllBytes( ScdUtils.VorbisHeader );

            using var writerMs = new MemoryStream();
            using var writer = new BinaryWriter( writerMs );
            writer.Write( rawHeader );
            writer.Write( oggData );

            writer.BaseStream.Seek( 0, SeekOrigin.Begin );
            writer.Write( oggData.Length - 0x10 ); // update data length
            writer.Write( oggReader.Channels );
            writer.Write( oggReader.SampleRate );

            writer.BaseStream.Seek( 0x10, SeekOrigin.Begin );
            writer.Write( 0 ); // loop start
            writer.Write( oggData.Length ); // loop end

            var newEntryData = writerMs.ToArray();

            using var readerMs = new MemoryStream( newEntryData );
            using var reader = new BinaryReader( readerMs );

            var newEntry = new ScdAudioEntry();
            newEntry.Read( reader );

            if( !string.IsNullOrEmpty( loopStartTag ) && int.TryParse( loopStartTag, out var loopStartSamples ) ) {
                newEntry.LoopStart = newEntry.Data.SamplesToBytes( loopStartSamples );
            }

            if( !string.IsNullOrEmpty( loopEndTag ) && int.TryParse( loopEndTag, out var loopEndSamples ) ) {
                newEntry.LoopEnd = newEntry.Data.SamplesToBytes( loopEndSamples );
            }

            Plugin.ScdManager.CurrentFile.Replace( oldEntry, newEntry );
            oldEntry.Dispose();
        }

        public static void ImportWav( string path, ScdAudioEntry entry ) {
            ScdUtils.ConvertToOgg( path );
            ImportOgg( ScdManager.ConvertOgg, entry );
        }
    }
}
