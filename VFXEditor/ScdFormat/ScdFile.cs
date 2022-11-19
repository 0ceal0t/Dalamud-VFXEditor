using Dalamud.Logging;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Lumina.Extensions;
using HelixToolkit.SharpDX.Core.Helper;

namespace VfxEditor.Data.Scd {
    public class ScdFile : Lumina.Data.FileResource {
        // https://github.com/goaaats/ffxiv-explorer-fork/blob/748028c3257f2c4010b63997993ea9af0061c4cc/src/main/java/com/fragmenterworks/ffxivextract/models/SCD_File.java
        // http://ffxivexplorer.fragmenterworks.com/research/scd%20files.txt
        // https://github.com/Soreepeong/xivres/blob/d35cf3ee55dbd9f3abc8f249c2972db94501edcf/xivres/include/xivres/sound.h
        // https://github.com/Sebane1/FFXIVVoiceClipNameGuesser/blob/main/FFXIVVoiceClipNameGuesser/SCDGenerator.cs

        // This is probably never getting done, is it?

        [StructLayout( LayoutKind.Sequential, Size = 0x30 )]
        public unsafe struct SoundHeader {
            public fixed char Sig[8];
            public int SedbVersion;
            public byte Endian; // little = 0, big = 1
            public byte SscfVersion;
            public short HeaderSize; // seems to always be 0x30
            public int FileSize;
            // padded to 0x30
        };

        [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
        public unsafe struct SoundOffsetsHeader {
            public short CountTable1;
            public short CountTable2;
            public short CountSound;
            public short Unk1;
            public int OffsetTable1;
            public int OffsetSound;
            public int OffsetTable2;
            public int Padding;
            public int UnkOffset;
            public int Unk2;
            // padded to 0x20
        }

        public SoundHeader Header;
        public SoundOffsetsHeader OffsetHeader;
        public List<EntrySound> Music = new();

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;
            Header = Reader.ReadStructure<SoundHeader>();
            OffsetHeader = Reader.ReadStructure<SoundOffsetsHeader>();

            // offsets lists 0/1/2/3/4
            // offset lists also padded to 16 bytes

            // In the file, table data is in order: 3, 0, 1, 4, 2
            // padded to 16 bytes between each table data (such as between 3/0)
            Reader.BaseStream.Seek( OffsetHeader.OffsetSound, SeekOrigin.Begin );
            for( var i = 0; i < OffsetHeader.CountSound; i++ ) {
                Music.Add( new EntrySound( Reader, Reader.ReadInt32() ) );
            }
        }

        public static void Test() {
            //Plugin.DataManager.GetFile<ScdFile>( "sound/vfx/se_vfx_common.scd" );
            //PluginLog.Log( "---------------------" );
            Plugin.DataManager.GetFile<ScdFile>( "sound/vfx/ability/se_vfx_abi_drk_bloodcontract_c.scd" );
        }
    }

    public abstract class ScdEntry {
        protected ScdEntry( BinaryReader reader, int offset ) {
            var oldPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            Read( reader );
            reader.BaseStream.Position = oldPosition;
        }

        protected ScdEntry( BinaryReader reader ) {
            Read( reader );
        }

        protected abstract void Read( BinaryReader reader );
    }

    public class EntrySound : ScdEntry {
        [StructLayout( LayoutKind.Sequential )]
        public unsafe struct Music_Header {
            public int NumChannels;
            public int Frequency;
            public int Type;
            public int LoopStart;
            public int LoopEnd;
            public int FirstFrame;
            public short AuxCount;
            public short Padding;
        };

        public Music_Header Header;
        public byte[] MusicData;

        public EntrySound( BinaryReader reader ) : base( reader ) { }
        public EntrySound( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            var dataLength = reader.ReadInt32();
            if( dataLength == 0 ) return; // placeholder or something

            Header = reader.ReadStructure<Music_Header>();

            var chunkStartPos = reader.BaseStream.Position;
            var chunkEndPos = chunkStartPos;
            for( var i = 0; i < Header.AuxCount; i++ ) {
                reader.ReadInt32(); // id
                chunkEndPos += reader.ReadInt32(); // data
                reader.BaseStream.Seek( chunkEndPos, SeekOrigin.Begin );
            }

            if( Header.Type == 0x06) {
                PluginLog.Log( $"ogg / vorbis" );

                var encodeMode = reader.ReadInt16();
                var encodeByte = reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                var seekTableSize = reader.ReadInt32();
                var vorbisHeaderSize = reader.ReadInt32();
                reader.ReadInt32();
                reader.BaseStream.Seek( chunkEndPos + 0x20 + seekTableSize, SeekOrigin.Begin );

                var vorbisHeader = reader.ReadBytes( vorbisHeaderSize );

                if( encodeMode == 0x2002 && encodeByte != 0x00 ) XorDecode( vorbisHeader, ( byte )encodeByte );

                var oggData = reader.ReadBytes( dataLength );
                MusicData = new byte[vorbisHeaderSize + dataLength];

                Buffer.BlockCopy( vorbisHeader, 0, MusicData, 0, vorbisHeaderSize );
                Buffer.BlockCopy( oggData, 0, MusicData, vorbisHeaderSize, dataLength );

                if( encodeMode == 0x2003 ) XorDecodeFromTable( MusicData, dataLength );
            }
            else if( Header.Type == 0x0C ) {
                PluginLog.Log( $"wav" );

                var wavHeader = reader.ReadBytes( 16 );
                reader.BaseStream.Seek( chunkStartPos + Header.FirstFrame, SeekOrigin.Begin );
                var data = reader.ReadBytes( dataLength );

                var wavFile = new byte[8 + 36 + dataLength];
                using var ms = new MemoryStream( wavFile );
                using var writer = new BinaryWriter( ms );
                writer.BaseStream.Seek( 0, SeekOrigin.Begin );
                writer.Write( 0x46464952 );
                writer.Write( 36 + dataLength );
                writer.Write( 0x45564157 );
                writer.Write( 0x20746D66 );
                writer.Write( 16 );
                writer.Write( wavHeader );
                writer.Write( 0x61746164 );
                writer.Write( dataLength );
                writer.Write( data );

                MusicData = wavFile;

                Play();
            }
        }

        public void Play() {
            if( Header.Type == 0x0C ) {
                new Thread( () => {
                    //using MemoryStream ms = new( MusicData );
                    //var reader = new WaveFileReader( ms );
                    var reader = new WaveFileReader( "C:\\Users\\kamin\\Downloads\\test\\music\\mine.wav" );

                    using( reader ) {
                        using var output = new DirectSoundOut() {
                            Volume = 1f
                        };

                        try {
                            output.Init( reader );
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

        private static void XorDecode( byte[] vorbisHeader, byte encodeByte ) {
            for( var i = 0; i < vorbisHeader.Length; i ++ ) {
                vorbisHeader[i] ^= encodeByte;
            }
        }

        private static void XorDecodeFromTable( byte[] dataFile, int dataLength ) {
            var byte1 = dataLength & 0xFF & 0x7F;
            var byte2 = byte1 & 0x3F;
            for( var i = 0; i < dataFile.Length; i++ ) {
                var xorByte = XORTABLE[( byte2 + i ) & 0xFF];
                xorByte &= 0xFF;
                xorByte ^= dataFile[i] & 0xFF;
                xorByte ^= byte1;
                dataFile[i] = ( byte )xorByte;
            }
        }

        private static readonly int[] XORTABLE = {0x003A, 0x0032, 0x0032, 0x0032, 0x0003, 0x007E, 0x0012,
            0x00F7, 0x00B2, 0x00E2, 0x00A2, 0x0067, 0x0032, 0x0032, 0x0022, 0x0032, 0x0032, 0x0052,
            0x0016, 0x001B, 0x003C, 0x00A1, 0x0054, 0x007B, 0x001B, 0x0097, 0x00A6, 0x0093, 0x001A,
            0x004B, 0x00AA, 0x00A6, 0x007A, 0x007B, 0x001B, 0x0097, 0x00A6, 0x00F7, 0x0002, 0x00BB,
            0x00AA, 0x00A6, 0x00BB, 0x00F7, 0x002A, 0x0051, 0x00BE, 0x0003, 0x00F4, 0x002A, 0x0051,
            0x00BE, 0x0003, 0x00F4, 0x002A, 0x0051, 0x00BE, 0x0012, 0x0006, 0x0056, 0x0027, 0x0032,
            0x0032, 0x0036, 0x0032, 0x00B2, 0x001A, 0x003B, 0x00BC, 0x0091, 0x00D4, 0x007B, 0x0058,
            0x00FC, 0x000B, 0x0055, 0x002A, 0x0015, 0x00BC, 0x0040, 0x0092, 0x000B, 0x005B, 0x007C,
            0x000A, 0x0095, 0x0012, 0x0035, 0x00B8, 0x0063, 0x00D2, 0x000B, 0x003B, 0x00F0, 0x00C7,
            0x0014, 0x0051, 0x005C, 0x0094, 0x0086, 0x0094, 0x0059, 0x005C, 0x00FC, 0x001B, 0x0017,
            0x003A, 0x003F, 0x006B, 0x0037, 0x0032, 0x0032, 0x0030, 0x0032, 0x0072, 0x007A, 0x0013,
            0x00B7, 0x0026, 0x0060, 0x007A, 0x0013, 0x00B7, 0x0026, 0x0050, 0x00BA, 0x0013, 0x00B4,
            0x002A, 0x0050, 0x00BA, 0x0013, 0x00B5, 0x002E, 0x0040, 0x00FA, 0x0013, 0x0095, 0x00AE,
            0x0040, 0x0038, 0x0018, 0x009A, 0x0092, 0x00B0, 0x0038, 0x0000, 0x00FA, 0x0012, 0x00B1,
            0x007E, 0x0000, 0x00DB, 0x0096, 0x00A1, 0x007C, 0x0008, 0x00DB, 0x009A, 0x0091, 0x00BC,
            0x0008, 0x00D8, 0x001A, 0x0086, 0x00E2, 0x0070, 0x0039, 0x001F, 0x0086, 0x00E0, 0x0078,
            0x007E, 0x0003, 0x00E7, 0x0064, 0x0051, 0x009C, 0x008F, 0x0034, 0x006F, 0x004E, 0x0041,
            0x00FC, 0x000B, 0x00D5, 0x00AE, 0x0041, 0x00FC, 0x000B, 0x00D5, 0x00AE, 0x0041, 0x00FC,
            0x003B, 0x0070, 0x0071, 0x0064, 0x0033, 0x0032, 0x0012, 0x0032, 0x0032, 0x0036, 0x0070,
            0x0034, 0x002B, 0x0056, 0x0022, 0x0070, 0x003A, 0x0013, 0x00B7, 0x0026, 0x0060, 0x00BA,
            0x001B, 0x0094, 0x00AA, 0x0040, 0x0038, 0x0000, 0x00FA, 0x00B2, 0x00E2, 0x00A2, 0x0067,
            0x0032, 0x0032, 0x0012, 0x0032, 0x00B2, 0x0032, 0x0032, 0x0032, 0x0032, 0x0075, 0x00A3,
            0x0026, 0x007B, 0x0083, 0x0026, 0x00F9, 0x0083, 0x002E, 0x00FF, 0x00E3, 0x0016, 0x007D,
            0x00C0, 0x001E, 0x0063, 0x0021, 0x0007, 0x00E3, 0x0001};
    }
}

