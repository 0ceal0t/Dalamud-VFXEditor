using Dalamud.Logging;
using Lumina.Extensions;
using NAudio.Vorbis;
using NAudio.Wave;
using NVorbis;
using SharpDX.Text;
using System;
using System.Collections.Generic;
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

        // https://github.com/Leinxad/KHPCSoundTools/blob/ad90c925c7b6b5d20fdabe0e7bc1c80bf106dbbe/SingleEncoder/Program.cs#L218
        private readonly SortedDictionary<int, int> PageBytesToSamples = new();
        private readonly static byte[] PagePattern = "OggS"u8.ToArray();
        private readonly static byte[] HeaderPattern = new byte[] { 0x05, 0x76, 0x6F, 0x72, 0x62, 0x69, 0x73 };
        private readonly int SampleRate;
        private readonly int FirstPageBytes;
        private readonly int LastPageSamples;

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

            // Parse out pages

            var headerOffset = Locate( DecodedData, HeaderPattern, 0, true );
            if( headerOffset == null || headerOffset.Length == 0 ) {
                PluginLog.Error( "Could not find header" );
            }
            var headerSize = headerOffset[0];

            var pageOffsets = Locate( DecodedData, PagePattern, headerSize, false );
            if( headerOffset == null || headerOffset.Length == 0 ) {
                PluginLog.Error( "Could not find pages" );
            }

            using var pageMs = new MemoryStream( DecodedData );
            using var decodedReader = new BinaryReader( pageMs );

            var lastPageSamples = 0;
            foreach( var offset in pageOffsets ) {
                decodedReader.BaseStream.Seek( offset + 6, SeekOrigin.Begin );
                var samples = decodedReader.ReadInt32();
                var pageOffset = offset + 4;
                if( PageBytesToSamples.Count == 0 ) FirstPageBytes = pageOffset;
                PageBytesToSamples[pageOffset] = samples;
                lastPageSamples = samples;
            }
            LastPageSamples = lastPageSamples;

            SampleRate = entry.SampleRate;
        }

        public override int TimeToBytes( float time ) => SamplesToBytes( ( int )( SampleRate * time ) );

        public override int SamplesToBytes( int samples ) {
            var lastBytes = 0;
            foreach( var page in PageBytesToSamples ) {
                if( page.Value == samples ) {
                    return Math.Min( page.Key, DecodedData.Length ) - FirstPageBytes;
                }
                else if( page.Value > samples ) {
                    return Math.Min( lastBytes, DecodedData.Length ) - FirstPageBytes;
                }
                lastBytes = page.Key;
            }
            return DecodedData.Length - FirstPageBytes;
        }

        public double BytesToTime( int bytes ) => ( ( double )BytesToSamples( bytes ) ) / SampleRate;

        public int BytesToSamples( int bytes ) {
            var lastSamples = 0;
            bytes += FirstPageBytes;
            foreach( var page in PageBytesToSamples ) {
                if( page.Key == bytes ) {
                    return page.Value;
                }
                else if( page.Key > bytes ) {
                    return lastSamples;
                }
                lastSamples = page.Value;
            }
            return LastPageSamples;
        }

        public override void BytesToLoopStartEnd( int loopStart, int loopEnd, out double startTime, out double endTime ) {
            if( loopStart == 0 && loopEnd == 0 ) {
                startTime = 0;
                endTime = 0;
                return;
            }

            startTime = BytesToTime( loopStart );
            endTime = BytesToTime( loopEnd );
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

        private static int[] Locate( byte[] data, byte[] candidate, int start, bool onlyOnce ) {
            if( IsEmptyLocate( data, candidate ) ) return null;

            var list = new List<int>();

            for( var i = start; i < data.Length; i++ ) {
                if( !IsMatch( data, i, candidate ) )
                    continue;

                list.Add( i );
                if( onlyOnce ) break;
            }

            return list.Count == 0 ? null : list.ToArray();
        }

        private static bool IsMatch( byte[] array, int position, byte[] candidate ) {
            if( candidate.Length > ( array.Length - position ) ) return false;

            for( var i = 0; i < candidate.Length; i++ )
                if( array[position + i] != candidate[i] ) return false;

            return true;
        }

        private static bool IsEmptyLocate( byte[] array, byte[] candidate ) {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
    }
}
