using DereTore.Exchange.Audio.HCA;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ScdFormat;
using VfxEditor.Formats.ScdFormat.Utils;

namespace VfxEditor.Formats.ScdFormat.Music.Data {
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
        private readonly byte[] RawData; // What will be written (header + encrypted frames)

        // 0x18 bytes of extra data, followed by the HCA header and frames
        private readonly byte Version = 0x02;
        private readonly byte ExtraSize = 0x18;
        private readonly short HeaderSize = 0x60;
        private readonly short BlockSize;
        private readonly short LoopStartBlock = 0; // always 0, the 32-bit ones are used instead
        private readonly short LoopEndBlock = 0;
        private readonly short InsertSamples; // same as the HCA fmt chunk
        private readonly byte UseMixer = 0;
        private readonly byte Encryption = 0; // always 0 in game files, even though the frames are XOR'd
        private readonly short Reserved = 0;
        // loop start + loop end, same as the entry

        private readonly DecodeParams DecodeParams = DecodeParams.Default;
        private readonly HcaInfo HcaInfo;
        private uint SamplesPerBlock => 0x80 * 8; // fixed 1024 samples/block, independent of channel count

        public ScdHca( byte[] data, HcaInfo info, ScdAudioEntry entry ) : base( entry ) {
            HeaderSize = GetHeaderSize( data );
            BlockSize = ( short )info.BlockSize;
            InsertSamples = GetInsertSamples( data );
            HcaInfo = info;

            var frames = data[HeaderSize..];
            Entry.DataLength = frames.Length;

            StreamData = data;
            RawData = [.. data[..HeaderSize], .. ScdUtils.XorDecodeFromTableHca( frames, BlockSize, frames.Length, HeaderSize )];
        }

        public ScdHca( BinaryReader reader, ScdAudioEntry entry ) : base( entry ) {
            Version = reader.ReadByte();
            ExtraSize = reader.ReadByte();
            HeaderSize = reader.ReadInt16();
            BlockSize = reader.ReadInt16();
            LoopStartBlock = reader.ReadInt16();
            LoopEndBlock = reader.ReadInt16();
            InsertSamples = reader.ReadInt16();
            UseMixer = reader.ReadByte();
            Encryption = reader.ReadByte();
            Reserved = reader.ReadInt16();
            reader.ReadInt32(); // loop start
            reader.ReadInt32(); // loop end

            var header = reader.ReadBytes( HeaderSize );
            var data = reader.ReadBytes( entry.DataLength );

            if( Encryption != 0 ) {
                Encryption = 0;
                Version = 0x02;
                InsertSamples = GetInsertSamples( header );
                data = data[..Math.Min( data.Length, ( int )( GetBlockCount( header ) * BlockSize ) )];
                Entry.DataLength = data.Length;

                StreamData = [.. header, .. data];
                RawData = [.. header, .. ScdUtils.XorDecodeFromTableHca( data, BlockSize, data.Length, HeaderSize )];
            }
            else {
                RawData = [.. header, .. data];
                StreamData = [.. header, .. ScdUtils.XorDecodeFromTableHca( data, BlockSize, entry.DataLength, HeaderSize )];
            }

            using var hcaMs = new MemoryStream( StreamData );
            using var decoder = new HcaDecoder( hcaMs, DecodeParams );
            HcaInfo = decoder.HcaInfo;
        }

        public override WaveStream GetStream() => new WaveFileReader( new HcaAudioStream( new MemoryStream( StreamData ), DecodeParams ) );

        public override void Write( BinaryWriter writer ) {
            writer.Write( Version );
            writer.Write( ExtraSize );
            writer.Write( HeaderSize );
            writer.Write( BlockSize );
            writer.Write( LoopStartBlock );
            writer.Write( LoopEndBlock );
            writer.Write( InsertSamples );
            writer.Write( UseMixer );
            writer.Write( Encryption );
            writer.Write( Reserved );
            writer.Write( Entry.LoopStart );
            writer.Write( Entry.LoopEnd );

            writer.Write( RawData );
        }

        // NOTE: for HCA, the SCD entry's LoopStart/LoopEnd are HCA block indices, not byte offsets
        // (matches the block indices found in the HCA file's own native "loop" chunk)
        public override int RawToSamples( int blocks ) => ( int )( blocks * SamplesPerBlock );

        public override int SamplesToRaw( int samples ) => ( int )Math.Round( ( float )samples / SamplesPerBlock, MidpointRounding.AwayFromZero );

        public override int TimeToRaw( float time ) {
            var samples = ( int )Math.Round( time * HcaInfo.SamplingRate, MidpointRounding.AwayFromZero );
            return SamplesToRaw( samples );
        }

        public override float RawToTime( int raw ) => ( float )RawToSamples( raw ) / HcaInfo.SamplingRate;

        public override Vector2 GetLoopTime() => new( RawToTime( Entry.LoopStart ), RawToTime( Entry.LoopEnd ) );

        public override int GetSubInfoSize() => HeaderSize + 0x18;

        public override Dictionary<string, GetAudioEntryDelegate> GetImportActions() => new() {
            ["wav"] = ImportWav,
            ["hca"] = ImportHca
        };

        public override string GetDefaultExtension() => "hca";

        public override byte[] GetDefaultExtensionData() => StreamData;

        // ====================

        private static ushort ReadUInt16BE( byte[] data, int offset ) => ( ushort )( ( data[offset] << 8 ) | data[offset + 1] );

        private static short GetHeaderSize( byte[] header ) => ( short )ReadUInt16BE( header, 0x06 );

        // "fmt" chunk always comes right after the "HCA" chunk
        private static uint GetBlockCount( byte[] header ) => ( ( uint )ReadUInt16BE( header, 0x10 ) << 16 ) | ReadUInt16BE( header, 0x12 );

        private static short GetInsertSamples( byte[] header ) => ( short )ReadUInt16BE( header, 0x14 );

        public static ScdAudioEntry ImportWav( string path, ScdAudioEntry oldEntry ) {
            using var waveReader = new WaveFileReader( path );

            if( waveReader.WaveFormat.Encoding != WaveFormatEncoding.Pcm ) {
                using var reader = new MediaFoundationReader( path );
                WaveFileWriter.CreateWaveFile( ScdManagerGroup.ConvertWav, reader ); // converted to pcm
            }
            else {
                File.Copy( path, ScdManagerGroup.ConvertWav, true );
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
                ( int )decoder.HcaInfo.ChannelCount,
                ( int )decoder.HcaInfo.SamplingRate,
                SscfWaveFormat.HCA
            );

            entry.Data = new ScdHca( data, decoder.HcaInfo, entry );

            // HCA files can carry their own native loop chunk (block indices), mirroring how OGG loop tags work
            if( decoder.HcaInfo.LoopFlag ) {
                entry.LoopStart = ( int )decoder.HcaInfo.LoopStart;
                entry.LoopEnd = ( int )decoder.HcaInfo.LoopEnd;
            }

            return entry;
        }
    }
}