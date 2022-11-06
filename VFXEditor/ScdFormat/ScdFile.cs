using Dalamud.Logging;
using Lumina.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.Data.Scd {
    public class ScdFile : Lumina.Data.FileResource {
        // https://github.com/goaaats/ffxiv-explorer-fork/blob/748028c3257f2c4010b63997993ea9af0061c4cc/src/main/java/com/fragmenterworks/ffxivextract/models/SCD_File.java
        // http://ffxivexplorer.fragmenterworks.com/research/scd%20files.txt
        // https://github.com/Soreepeong/xivres/blob/d35cf3ee55dbd9f3abc8f249c2972db94501edcf/xivres/include/xivres/sound.h

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
            public int OffsetTable2;
            public int OffsetSound;
            public int OffsetTable4;
            public int Padding;
            public int OffsetTable5;
            public int Unk2;
            // padded to 0x20
        }

        public SoundHeader Header;
        public SoundOffsetsHeader OffsetHeader;
        public List<EntryTable1> Table0 = new();
        public List<EntryCamera> Camera = new();
        public List<EntrySound> Music = new();

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;
            Header = Reader.ReadStructure<SoundHeader>();
            OffsetHeader = Reader.ReadStructure<SoundOffsetsHeader>();

            // padded out to multiples of 4
            var countTable1 = RoundUp( OffsetHeader.CountTable1 );
            var countTable2 = RoundUp( OffsetHeader.CountTable2 );
            var countSound = RoundUp( OffsetHeader.CountSound );
            var countTable4 = countTable1;
            var countTable5 = countTable1; // maybe subtract position from header size?

            // In the file, table data is in order: 3, 0, 1, 4, 2
            // padded to 16 bytes between each table data (such as between 3/0)

            // ============ TABLE 0 ===============
            for( var i = 0; i < countTable1; i++ ) {
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"0: {Reader.BaseStream.Position:X8} -> {offset:X8}" );

                Table0.Add( new EntryTable1( Reader, offset ) );
            }
            // ============ TABLE 1 ===============
            for( var i = 0; i < countTable2; i++ ) {
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"1: {Reader.BaseStream.Position:X8} -> {offset:X8}" );
            }
            // ============ TABLE 2 ===============
            for( var i = 0; i < countSound; i++ ) { // Sound effect entries
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"2: {Reader.BaseStream.Position:X8} -> {offset:X8}" );

                Music.Add( new EntrySound( Reader, offset ) );
            }
            // ============ TABLE 3 ===============
            for( var i = 0; i < countTable4; i++ ) { // Camera control
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"3: {Reader.BaseStream.Position:X8} -> {offset:X8}" );

                Camera.Add( new EntryCamera( Reader, offset ) );
            }
            // ============ TABLE 4 ===============
            for( var i = 0; i < countTable5; i++ ) {
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"4: {Reader.BaseStream.Position:X8} -> {offset:X8}" );
            }
        }

        private static int RoundUp( int value, int round = 4 ) => round * ( int )Math.Ceiling( value / ( float )round );

        public static void Test() {
            Plugin.DataManager.GetFile<ScdFile>( "sound/vfx/se_vfx_common.scd" );
            PluginLog.Log( "---------------------" );
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

    public class EntryTable1 : ScdEntry {
        [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
        public unsafe struct Table0_Header {
            public byte UnkCount;
            public byte Unk1;
            public short Unk2;
            public short Unk3;
            public short Unk4;
            public float Unk5;
            public int Unk6;
            // TODO:
            // 8 shorts
            // 2*UnkCount shorts
        }

        public Table0_Header Header;

        public EntryTable1( BinaryReader reader ) : base( reader ) { }
        public EntryTable1( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Header = reader.ReadStructure<Table0_Header>();
        }
    }

    public class EntrySound : ScdEntry {
        [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
        public unsafe struct Music_Header {
            public int Length;
            public int NumChannels;
            public int Frequency;
            public int Type;
            public int LoopStart;
            public int LoopEnd;
            public int FirstFrame;
            public short AuxCount;
            // padded to 0x20
        };

        public struct AuxChunk {
            public int Id;
            public byte[] Data;
        }

        public Music_Header Header;
        public List<AuxChunk> AuxChunks = new();

        public EntrySound( BinaryReader reader ) : base( reader ) { }
        public EntrySound( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Header = reader.ReadStructure<Music_Header>();

            for( var i = 0; i < Header.AuxCount; i++ ) {
                var id = reader.ReadInt32();
                var chunkSize = reader.ReadInt32();
                var data = reader.ReadBytes( chunkSize - 8 ); // 4 bytes for id, 4 for size
                AuxChunks.Add( new AuxChunk {
                    Id = id,
                    Data = data
                } );
            }

            // TODO: data types

            PluginLog.Log( $"TABLE 2: {Header.Length} {Header.Type} {Header.AuxCount}" );
        }
    }

    public class EntryCamera : ScdEntry {
        [StructLayout( LayoutKind.Explicit, Size = 0x80 )]
        public unsafe struct Camera_Data {
            [FieldOffset( 0x00 )] public short Size; // seems to always be 0x80
            [FieldOffset( 0x02 )] public short Unk1;
            [FieldOffset( 0x04 )] public short Unk2;

            [FieldOffset( 0x10 )] public fixed float Matrix[16];

            [FieldOffset( 0x74 )] public short Unk3;
            [FieldOffset( 0x76 )] public short Unk4;
        };

        public Camera_Data Data;

        public EntryCamera( BinaryReader reader ) : base( reader ) { }
        public EntryCamera( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Data = reader.ReadStructure<Camera_Data>();

            PluginLog.Log( $"TABLE 3: {Data.Size} {Data.Unk1} {Data.Unk2} {Data.Unk3} {Data.Unk4}" );
        }
    }
}

