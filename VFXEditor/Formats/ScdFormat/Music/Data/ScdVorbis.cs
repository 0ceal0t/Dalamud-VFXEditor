using NAudio.Vorbis;
using NAudio.Wave;
using NVorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using VfxEditor.Formats.ScdFormat.Utils;

namespace VfxEditor.ScdFormat.Music.Data {
    public class ScdVorbis : ScdAudioData {
        public readonly bool LegacyImported = false; // old scds

        private readonly short EncodeMode = 0; // default to unencoded
        private readonly short EncodeByte = 0;
        private readonly int XorOffset = 0;
        private readonly int XorSize = 0;
        private readonly int Unknown1 = 0;
        private readonly int Unknown2 = 0;

        private float SeekStep = 0.1f;
        private readonly List<int> SeekTable = new(); // how many bytes to get to each `SeekStep`

        private readonly int VorbisHeaderSize = 0;
        private readonly byte[] EncodedData;
        private readonly byte[] DecodedData;
        public readonly byte[] Data; // final decoded

        // https://github.com/CrimsonOrion/CS-FFXIV-Data-Worker/blob/af126fd74139f54722b4dd8feea87c54077caeb7/FFXIV%20Data%20Worker/OggToScd.cs
        // https://github.com/Soreepeong/XivAlexander/blob/0b1077ebbcd2bf13955169fddc2bc38c218d19fe/XivAlexanderCommon/Sqex/Sound/Writer.cpp#L63
        // https://github.com/Leinxad/KHPCSoundTools/blob/ad90c925c7b6b5d20fdabe0e7bc1c80bf106dbbe/SingleEncoder/Program.cs#L218

        // https://en.wikipedia.org/wiki/Ogg#Page_structure
        // "OggS" Page pattern with version, header type, and grandule position = 0
        private static readonly byte[] PagePattern = new byte[] { 0x4F, 0x67, 0x67, 0x53, 0x00 };

        public ScdVorbis( byte[] data, ScdAudioEntry entry ) : base( entry ) {
            EncodedData = Array.Empty<byte>();
            DecodedData = data;
            Data = DecodedData;
            Entry.DataLength = Data.Length;

            PopualateSeekTable( Data );
        }

        public ScdVorbis( BinaryReader reader, ScdAudioEntry entry ) : base( entry ) {
            EncodeMode = reader.ReadInt16();
            EncodeByte = reader.ReadInt16();
            XorOffset = reader.ReadInt32();
            XorSize = reader.ReadInt32();
            SeekStep = reader.ReadSingle();
            var seekTableSize = reader.ReadInt32();

            // ====== LEGACY SCD STUFF =======
            if( Encoding.ASCII.GetString( BitConverter.GetBytes( seekTableSize ) ).EndsWith( "vor" ) ) {
                LegacyImported = true;

                EncodeMode = 0;
                EncodeByte = 0;
                XorOffset = 0;
                XorSize = 0;
                SeekStep = 0.1f;

                reader.ReadBytes( 0x35C ); // skip legacy header
                EncodedData = Array.Empty<byte>();
                DecodedData = reader.ReadBytes( entry.DataLength + 0x10 );
                Data = DecodedData;
                Entry.DataLength = Data.Length;

                PopualateSeekTable( Data );

                return;
            }
            // ===============================

            VorbisHeaderSize = reader.ReadInt32();
            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt32();
            for( var i = 0; i < seekTableSize / 4; i++ ) SeekTable.Add( reader.ReadInt32() );

            EncodedData = reader.ReadBytes( VorbisHeaderSize );
            var decodedHeader = new byte[EncodedData.Length];
            Buffer.BlockCopy( EncodedData, 0, decodedHeader, 0, decodedHeader.Length );
            if( EncodeMode == 0x2002 && EncodeByte != 0x00 ) ScdUtils.XorDecode( decodedHeader, ( byte )EncodeByte );

            DecodedData = reader.ReadBytes( entry.DataLength );
            using( var ms = new MemoryStream() )
            using( var writer = new BinaryWriter( ms ) ) {
                writer.Write( decodedHeader );
                writer.Write( DecodedData );
                Data = ms.ToArray();
            }
            if( EncodeMode == 0x2003 ) ScdUtils.XorDecodeFromTable( Data, DecodedData.Length );
        }

        private void PopualateSeekTable( byte[] data ) {
            var candidates = Locate( Data, PagePattern, 0, false );
            using var ms = new MemoryStream( Data );
            using var dataReader = new BinaryReader( ms );
            foreach( var offset in candidates ) {
                var pos = offset - VorbisHeaderSize;
                if( pos < 0 ) continue;
                dataReader.BaseStream.Position = offset + 6;

                var maxSamples = dataReader.ReadInt32();
                var maxTime = ( float )maxSamples / Entry.SampleRate;

                if( SeekTable.Count == 1 && maxTime > SeekStep ) {
                    SeekStep = maxTime;
                    Dalamud.Log( $"SeekStep is now: {SeekStep} seconds" );
                }

                if( ( ( SeekStep * SeekTable.Count ) - maxTime ) < 0.02f ) SeekTable.Add( pos );
            }
        }

        public override int TimeToBytes( float time ) {
            if( SeekTable.Count == 0 ) return 0;
            for( var i = 0; i < SeekTable.Count; i++ ) {
                if( i * SeekStep > time ) return SeekTable[i - 1];
            }
            return Data.Length - VorbisHeaderSize;
        }

        public override int SamplesToBytes( int samples ) => TimeToBytes( samples / Entry.SampleRate );

        public double BytesToTime( int bytes ) {
            if( SeekTable.Count == 0 ) return 0;
            for( var i = 0; i < SeekTable.Count; i++ ) {
                if( SeekTable[i] > bytes ) return SeekStep * ( i - 1 );
            }
            return SeekTable.Count * SeekStep;
        }

        public override Vector2 GetLoopTime() {
            if( Entry.LoopStart == 0 && Entry.LoopEnd == 0 ) return new( 0, 0 );
            return new( ( float )BytesToTime( Entry.LoopStart ), ( float )BytesToTime( Entry.LoopEnd ) );
        }

        public override WaveStream GetStream() {
            var ms = new MemoryStream( Data, 0, Data.Length, false );
            return new VorbisWaveReader( ms );
        }

        public override void Write( BinaryWriter writer ) {
            writer.Write( EncodeMode );
            writer.Write( EncodeByte );
            writer.Write( XorOffset );
            writer.Write( XorSize );
            writer.Write( SeekStep );
            writer.Write( SeekTable.Count * 4 );
            writer.Write( VorbisHeaderSize );
            writer.Write( Unknown1 );
            writer.Write( Unknown2 );
            foreach( var item in SeekTable ) writer.Write( item );
            writer.Write( EncodedData );
            writer.Write( DecodedData );
        }

        public override int GetSubInfoSize() => 0x20 + ( SeekTable.Count * 4 ) + VorbisHeaderSize;

        // =======================

        public static ScdAudioEntry ImportOgg( string path, ScdAudioEntry oldEntry ) {
            using var oggReader = new VorbisReader( path );
            var loopStartTag = oggReader.Tags.GetTagSingle( "LoopStart" );
            var loopEndTag = oggReader.Tags.GetTagSingle( "LoopEnd" );

            var oggData = File.ReadAllBytes( path );

            // Create new entry
            var entry = new ScdAudioEntry(
                oldEntry,
                0, // data length is a placeholder
                oggReader.Channels,
                oggReader.SampleRate,
                SscfWaveFormat.Vorbis
            );

            // Create new data
            var vorbis = new ScdVorbis( oggData, entry );
            if( !string.IsNullOrEmpty( loopStartTag ) && int.TryParse( loopStartTag, out var loopStartSamples ) ) {
                entry.LoopStart = vorbis.SamplesToBytes( loopStartSamples );
            }
            if( !string.IsNullOrEmpty( loopEndTag ) && int.TryParse( loopEndTag, out var loopEndSamples ) ) {
                entry.LoopEnd = vorbis.SamplesToBytes( loopEndSamples );
            }

            entry.Data = vorbis;
            return entry;
        }

        public static ScdAudioEntry ImportWav( string path, ScdAudioEntry oldEntry ) {
            ScdUtils.ConvertWavToOgg( path );
            return ImportOgg( ScdManager.ConvertOgg, oldEntry );
        }

        // ================================

        private static int[] Locate( byte[] data, byte[] candidate, int start, bool onlyOnce ) {
            if( IsEmptyLocate( data, candidate ) ) return null;

            var list = new List<int>();

            for( var i = start; i < data.Length; i++ ) {
                if( !IsMatch( data, i, candidate ) ) continue;
                list.Add( i );
                if( onlyOnce ) break;
            }

            return list.Count == 0 ? null : list.ToArray();
        }

        private static bool IsMatch( byte[] array, int position, byte[] candidate ) {
            if( candidate.Length > ( array.Length - position ) ) return false;

            for( var i = 0; i < candidate.Length; i++ ) {
                if( array[position + i] != candidate[i] ) return false;
            }

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
