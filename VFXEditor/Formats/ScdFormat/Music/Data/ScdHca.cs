using DereTore.Exchange.Audio.HCA;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ScdFormat;
using VfxEditor.Formats.ScdFormat.Utils;

namespace VfxEditor.ScdFormat.Music.Data {
    public class ScdHca : ScdAudioData {
        // Decoding based on Wintermute's implementation


        // TODO: CRC right before the end of the header + data
        /*
            if you crc the whole header
            it'll give you 0
            (because they just crc everything up until the last 2 bytes, then whatever it returns, they write the last 2 bytes as it)
            (that's what that f8 2c is for)
         */

        private readonly byte[] StreamData; // Decoded
        private readonly byte[] RawData; // What will be written, can be the same if there's no encryption

        private readonly short HeaderSize = 0x60;
        private readonly int BlockSize;
        private readonly bool PlainText = true;

        private readonly DecodeParams DecodeParams = DecodeParams.Default;
        private readonly HcaInfo HcaInfo;
        private uint SamplesPerBlock => HcaInfo.ChannelCount * 0x80 * 8;

        private readonly byte[] Unk1 = [ 0x20, 0x18 ];
        private readonly byte[] Unk2 = [ 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00 ];
        private readonly byte[] Unk3 = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ];

        public ScdHca( byte[] data, HcaInfo info, ScdAudioEntry entry ) : base( entry ) {
            StreamData = data;
            RawData = data;
            HcaInfo = info;
            BlockSize = ( int )info.BlockSize;
            Entry.DataLength = data.Length;
        }

        public ScdHca( BinaryReader reader, ScdAudioEntry entry ) : base( entry ) {
            Unk1 = reader.ReadBytes(2); // TODO
            HeaderSize = reader.ReadInt16();
            BlockSize = reader.ReadInt16();
            Unk2 = reader.ReadBytes( 7 ); // TODO
            PlainText = reader.ReadBoolean();
            Unk3 = reader.ReadBytes( 10 ); // TODO

            using var streamMs = new MemoryStream();
            using var streamWriter = new BinaryWriter( streamMs );

            using var rawMs = new MemoryStream();
            using var rawWriter = new BinaryWriter( rawMs );

            var header = reader.ReadBytes( HeaderSize );
            var data = reader.ReadBytes( entry.DataLength );

            if( !PlainText ) {
                streamWriter.Write( header );
                streamWriter.Write( ScdUtils.XorDecodeFromTableHca( data, BlockSize, entry.DataLength, HeaderSize ) );
                streamWriter.BaseStream.Position = 0;
            }

            rawWriter.Write( header );
            rawWriter.Write( data );

            RawData = rawMs.ToArray();
            StreamData = PlainText ? RawData : streamMs.ToArray();

            using var hcaMs = new MemoryStream( StreamData );
            using var decoder = new HcaDecoder( hcaMs, DecodeParams );
            HcaInfo = decoder.HcaInfo;
        }

        public override WaveStream GetStream() => new WaveFileReader( new HcaAudioStream( new MemoryStream( StreamData ), DecodeParams ) );

        public override void Write( BinaryWriter writer ) {
            writer.Write( Unk1 );
            writer.Write( HeaderSize );
            writer.Write( (short) BlockSize );
            writer.Write( Unk2 );
            writer.Write( PlainText );
            writer.Write( Unk3 );

            writer.Write( RawData );
        }

        public override int BytesToSamples( int bytes ) {
            var blockCount = bytes / HcaInfo.BlockSize;
            return ( int )( blockCount * SamplesPerBlock );
        }

        public override int SamplesToBytes( int samples ) {
            var targetBlock = ( int )Math.Round( ( float )samples / SamplesPerBlock, MidpointRounding.AwayFromZero );
            return ( int )( targetBlock * HcaInfo.BlockSize );
        }

        public override int TimeToBytes( float time ) {
            var samples = ( int )Math.Round( time * HcaInfo.SamplingRate, MidpointRounding.AwayFromZero );
            return SamplesToBytes( samples );
        }

        public override float BytesToTime( int bytes ) => ( float )BytesToSamples( bytes ) / HcaInfo.SamplingRate;

        public override Vector2 GetLoopTime() => new( BytesToTime( Entry.LoopStart ), BytesToTime( Entry.LoopEnd ) );

        public override int GetSubInfoSize() => HeaderSize + 0x18;

        public override Dictionary<string, GetAudioEntryDelegate> GetImportActions() => new() {
            ["wav"] = ImportWav,
            ["hca"] = ImportHca
        };

        public override string GetDefaultExtension() => "hca";

        public override byte[] GetDefaultExtensionData() => StreamData;

        // ====================

        public static ScdAudioEntry ImportWav( string path, ScdAudioEntry oldEntry ) {
            using var waveReader = new WaveFileReader( path );

            if( waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm ) {
                using var reader = new MediaFoundationReader( path );
                WaveFileWriter.CreateWaveFile( ScdManagerGroup.ConvertWav, reader ); // converted to pcm
            }
            else {
                File.Copy( path, ScdManagerGroup.ConvertWav ); // already good
            }

            ScdUtils.ConvertToHca( ScdManagerGroup.ConvertWav );
            Dalamud.Log( $"Finished converting to HCA: {ScdManagerGroup.ConvertHca}" );
            return ImportHca( ScdManagerGroup.ConvertHca, oldEntry );
        }

        public static ScdAudioEntry ImportHca( string path, ScdAudioEntry oldEntry ) {
            var data = File.ReadAllBytes( path );
            using var ms = new MemoryStream( data );
            using var decoder = new HcaDecoder( ms );

            // Create new entry
            var entry = new ScdAudioEntry(
                oldEntry,
                0, // data length is a placeholder
                (int) decoder.HcaInfo.ChannelCount,
                (int) decoder.HcaInfo.SamplingRate,
                SscfWaveFormat.HCA
            );

            entry.Data = new ScdHca( data, decoder.HcaInfo, entry );

            // HCA files can carry their own native loop chunk (block indices), mirroring how OGG loop tags work
            if( decoder.HcaInfo.LoopFlag ) {
                entry.LoopStart = ( int )( decoder.HcaInfo.LoopStart * decoder.HcaInfo.BlockSize );
                entry.LoopEnd = ( int )( decoder.HcaInfo.LoopEnd * decoder.HcaInfo.BlockSize );
            }

            return entry;
        }
    }
}
