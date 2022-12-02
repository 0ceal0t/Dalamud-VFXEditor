using Dalamud.Logging;
using NAudio.Wave;
using System.IO;

namespace VfxEditor.ScdFormat {
    public class ScdAdpcm : ScdAudioData {
        public byte[] WaveHeader;
        public byte[] Data;
        public WaveFormat Format;

        public ScdAdpcm( BinaryReader reader, ScdAudioEntry entry ) {
            WaveHeader = reader.ReadBytes( entry.FirstFrame - entry.AuxChunkData.Length );
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

        public static void Import( string path, ScdAudioEntry entry ) {
            var waveFileCheck = new WaveFileReader( path );
            if( waveFileCheck.WaveFormat.Encoding == WaveFormatEncoding.Adpcm ) {
                PluginLog.Log( "Already Adpcm, skipping conversion" );
                File.Copy( path, ScdManager.ConvertWav, true );
            }
            else {
                ScdUtils.ConvertToAdpcm( path );
            }
            waveFileCheck.Close();

            if( !File.Exists( ScdManager.ConvertWav ) ) {
                PluginLog.Error( "Could not conver to ADPCM" );
                return;
            }

            var data = ( ScdAdpcm )entry.Data;
            using var waveFile = new WaveFileReader( ScdManager.ConvertWav );

            var rawData = File.ReadAllBytes( ScdManager.ConvertWav );
            var waveFormat = waveFile.WaveFormat;

            using var ms = new MemoryStream( rawData );
            using var br = new BinaryReader( ms );
            br.ReadInt32(); // RIFF
            br.ReadInt32();
            br.ReadInt32(); // WAVE
            br.ReadInt32(); // fmt
            var headerLength = br.ReadInt32();
            data.WaveHeader = br.ReadBytes( headerLength );

            var magic = br.ReadInt32();
            while( magic != 0x61746164 ) { // data
                var size = br.ReadInt32();
                br.ReadBytes( size );
                magic = br.ReadInt32();
            }
            var dataLength = br.ReadInt32();
            data.Data = br.ReadBytes( dataLength );

            data.Format = waveFormat;
            entry.DataLength = dataLength;
            entry.FirstFrame = headerLength + entry.AuxChunkData.Length;
            entry.SampleRate = waveFormat.SampleRate;
            entry.NumChannels = waveFormat.Channels;
            entry.BitsPerSample = ( short )waveFormat.BitsPerSample;
        }
    }
}
