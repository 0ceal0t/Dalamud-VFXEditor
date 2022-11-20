using Dalamud.Logging;
using ImGuiNET;
using NAudio.Wave;
using System;
using System.IO;
using System.Reflection.PortableExecutable;
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

        public ScdSoundEntry( BinaryReader reader ) : base( reader ) { }
        public ScdSoundEntry( BinaryReader reader, int offset ) : base( reader, offset ) { }

        public ScdSoundData Data;

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
                _ => null
            };
        }

        public void Draw( string id ) {
            if( ImGui.Button( $"Play{id}" ) ) {
                new Thread( () => {
                    var stream = Data.GetStream();
                    var converted = stream.WaveFormat.Encoding switch {
                        WaveFormatEncoding.Pcm => WaveFormatConversionStream.CreatePcmStream( stream ),
                        WaveFormatEncoding.Adpcm => WaveFormatConversionStream.CreatePcmStream( stream ),
                        _ => stream
                    };
                    using var channel = new WaveChannel32( converted ) {
                        Volume = 1f,
                        PadWithZeroes = false,
                    };

                    using( converted ) {
                        using var output = new WasapiOut();

                        try {
                            output.Init( channel );
                            output.Play();

                            while( output.PlaybackState == PlaybackState.Playing ) {
                                Thread.Sleep( 500 );
                            }
                        }
                        catch( Exception e ) {
                            PluginLog.LogError( e, "Error playing sound" );
                        }
                    }

                } ).Start();
            }
        }
    }
}
