using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ScdFormat;
using VfxEditor.Formats.ScdFormat.Utils;

namespace VfxEditor.ScdFormat.Music.Data {
    public class ScdAdpcm : ScdAudioData {
        public readonly WaveFormat Format;
        public readonly byte[] WaveHeader;
        public readonly byte[] Data;

        // Standard MS-ADPCM block-align/samples-per-block relationship (4 bits/sample, 7-byte header per channel)
        private readonly int SamplesPerBlock;

        public ScdAdpcm( WaveFormat format, byte[] waveHeader, byte[] data, ScdAudioEntry entry ) : base( entry ) {
            Format = format;
            WaveHeader = waveHeader;
            Data = data;
            SamplesPerBlock = ( ( Format.BlockAlign - ( 7 * Format.Channels ) ) * 2 / Format.Channels ) + 2;
        }

        public ScdAdpcm( BinaryReader reader, int headerSize, ScdAudioEntry entry ) : base( entry ) {
            WaveHeader = reader.ReadBytes( headerSize );
            Data = reader.ReadBytes( entry.DataLength );

            using var ms = new MemoryStream( WaveHeader );
            using var br = new BinaryReader( ms );
            Format = WaveFormat.FromFormatChunk( br, WaveHeader.Length );
            SamplesPerBlock = ( ( Format.BlockAlign - ( 7 * Format.Channels ) ) * 2 / Format.Channels ) + 2;
        }

        public override WaveStream GetStream() => new RawSourceWaveStream( new MemoryStream( Data, 0, Data.Length, false ), Format );

        public override void Write( BinaryWriter writer ) {
            writer.Write( WaveHeader );
            writer.Write( Data );
        }

        // MS-ADPCM blocks carry predictor state, so loop points must snap to a block boundary to decode correctly
        public override int BytesToSamples( int bytes ) {
            var block = bytes / Format.BlockAlign;
            return block * SamplesPerBlock;
        }

        public override int SamplesToBytes( int samples ) {
            var block = ( int )Math.Round( ( float )samples / SamplesPerBlock, MidpointRounding.AwayFromZero );
            return block * Format.BlockAlign;
        }

        public override int TimeToBytes( float time ) => SamplesToBytes( ( int )Math.Round( time * Entry.SampleRate, MidpointRounding.AwayFromZero ) );

        public override float BytesToTime( int bytes ) => ( float )BytesToSamples( bytes ) / Entry.SampleRate;

        public override Vector2 GetLoopTime() => new( BytesToTime( Entry.LoopStart ), BytesToTime( Entry.LoopEnd ) );

        public override int GetSubInfoSize() => WaveHeader.Length;

        public override Dictionary<string, GetAudioEntryDelegate> GetImportActions() => new() {
            ["wav"] = ImportWav,
            ["ogg"] = ScdVorbis.ImportOgg
        };

        public override string GetDefaultExtension() => "wav";

        public override byte[] GetDefaultExtensionData() => Data;

        // ===================

        public static ScdAudioEntry ImportWav( string path, ScdAudioEntry oldEntry ) {
            var waveFileCheck = new WaveFileReader( path );
            if( waveFileCheck.WaveFormat.Encoding == WaveFormatEncoding.Adpcm ) {
                Dalamud.Log( "Already Adpcm, skipping conversion" );
                File.Copy( path, ScdManagerGroup.ConvertWav, true );
            }
            else {
                ScdUtils.ConvertToAdpcm( path );
            }
            waveFileCheck.Close();

            if( !File.Exists( ScdManagerGroup.ConvertWav ) ) {
                Dalamud.Error( "Could not convert to ADPCM" );
                return null;
            }

            using var waveFile = new WaveFileReader( ScdManagerGroup.ConvertWav );
            var rawData = File.ReadAllBytes( ScdManagerGroup.ConvertWav );
            var format = waveFile.WaveFormat;

            using var ms = new MemoryStream( rawData );
            using var br = new BinaryReader( ms );
            br.ReadInt32(); // RIFF
            br.ReadInt32();
            br.ReadInt32(); // WAVE
            br.ReadInt32(); // fmt
            var headerLength = br.ReadInt32();
            var waveHeader = br.ReadBytes( headerLength );
            var magic = br.ReadInt32();
            while( magic != 0x61746164 ) { // data
                var size = br.ReadInt32();
                br.ReadBytes( size );
                magic = br.ReadInt32();
            }
            var dataLength = br.ReadInt32();
            var data = br.ReadBytes( dataLength );

            // Create new entry
            var entry = new ScdAudioEntry(
                oldEntry,
                dataLength,
                format.Channels,
                format.SampleRate,
                SscfWaveFormat.MsAdPcm
            );

            // Create new data
            var adpcm = new ScdAdpcm( format, waveHeader, data, entry );

            entry.Data = adpcm;
            return entry;
        }
    }
}
