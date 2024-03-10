using NAudio.Wave;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.ScdFormat.Utils;

namespace VfxEditor.ScdFormat.Music.Data {
    public class ScdAdpcm : ScdAudioData {
        public readonly WaveFormat Format;
        public readonly byte[] WaveHeader;
        public readonly byte[] Data;

        public ScdAdpcm( WaveFormat format, byte[] waveHeader, byte[] data, ScdAudioEntry entry ) : base( entry ) {
            Format = format;
            WaveHeader = waveHeader;
            Data = data;
        }

        public ScdAdpcm( BinaryReader reader, int headerSize, ScdAudioEntry entry ) : base( entry ) {
            WaveHeader = reader.ReadBytes( headerSize );
            Data = reader.ReadBytes( entry.DataLength );

            using var ms = new MemoryStream( WaveHeader );
            using var br = new BinaryReader( ms );
            Format = WaveFormat.FromFormatChunk( br, WaveHeader.Length );
        }

        public override WaveStream GetStream() {
            var ms = new MemoryStream( Data, 0, Data.Length, false );
            return new RawSourceWaveStream( ms, Format );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( WaveHeader );
            writer.Write( Data );
        }

        public override int SamplesToBytes( int samples ) => TimeToBytes( samples / Entry.SampleRate );

        public override int TimeToBytes( float time ) => ( int )( Format.AverageBytesPerSecond * time );

        public float BytesToTime( int bytes ) => ( float )bytes / Format.AverageBytesPerSecond;

        public override Vector2 GetLoopTime() => new( BytesToTime( Entry.LoopStart ), BytesToTime( Entry.LoopEnd ) );

        public override int GetSubInfoSize() => WaveHeader.Length;

        // ===================

        public static ScdAudioEntry ImportWav( string path, ScdAudioEntry oldEntry ) {
            var waveFileCheck = new WaveFileReader( path );
            if( waveFileCheck.WaveFormat.Encoding == WaveFormatEncoding.Adpcm ) {
                Dalamud.Log( "Already Adpcm, skipping conversion" );
                File.Copy( path, ScdManager.ConvertWav, true );
            }
            else {
                ScdUtils.ConvertToAdpcm( path );
            }
            waveFileCheck.Close();

            if( !File.Exists( ScdManager.ConvertWav ) ) {
                Dalamud.Error( "Could not conver to ADPCM" );
                return null;
            }

            using var waveFile = new WaveFileReader( ScdManager.ConvertWav );
            var rawData = File.ReadAllBytes( ScdManager.ConvertWav );
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
