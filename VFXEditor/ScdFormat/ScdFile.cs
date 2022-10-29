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

        [StructLayout( LayoutKind.Sequential, Size = 0x30 )]
        public unsafe struct SoundHeader {
            public fixed char Sig[8];
            public int Version;
            public short Unk1;
            public short HeaderSize; // seems to always be 0x30
            public int FileSize;
            // padded to 0x30
        };

        [StructLayout( LayoutKind.Sequential, Size = 0x20 )]
        public unsafe struct SoundOffsetsHeader {
            public short Table0_Count;
            public short Table1_Count;
            public short Table2_Count;
            public short Unk1;
            public int Table1_Offset;
            public int Table2_Offset;
            public int Table3_Offset;
            public int Table4_Offset;
            public int Unk2;
            // padded to 0x20
        }

        public SoundHeader Header;
        public SoundOffsetsHeader OffsetHeader;
        public List<Table0Entry> Table0 = new();
        public List<CameraEntry> Camera = new();
        public List<MusicEntry> Music = new();

        public override void LoadFile() {
            PluginLog.Log( $"Length: {Reader.BaseStream.Length}" );

            Reader.BaseStream.Position = 0;
            Header = Reader.ReadStructure<SoundHeader>();
            OffsetHeader = Reader.ReadStructure<SoundOffsetsHeader>();

            // padded out to multiples of 4
            var table0_Count = RoundUp( OffsetHeader.Table0_Count );
            var table1_Count = RoundUp( OffsetHeader.Table1_Count );
            var table2_Count = RoundUp( OffsetHeader.Table2_Count );
            var table3_Count = table0_Count;
            var table4_Count = table0_Count;

            // In the file, table data is in order: 3, 0, 1, 4, 2
            // padded to 16 bytes between each table data (such as between 3/0)

            // ============ TABLE 0 ===============
            for( var i = 0; i < table0_Count; i++ ) {
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"0: {Reader.BaseStream.Position:X8} -> {offset:X8}" );

                Table0.Add( new Table0Entry( Reader, offset ) );
            }
            // ============ TABLE 1 ===============
            for( var i = 0; i < table1_Count; i++ ) {
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"1: {Reader.BaseStream.Position:X8} -> {offset:X8}" );
            }
            // ============ TABLE 2 ===============
            for( var i = 0; i < table2_Count; i++ ) { // Sound effect entries
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"2: {Reader.BaseStream.Position:X8} -> {offset:X8}" );

                Music.Add( new MusicEntry( Reader, offset ) );
            }
            // ============ TABLE 3 ===============
            for( var i = 0; i < table3_Count; i++ ) { // Camera control
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"3: {Reader.BaseStream.Position:X8} -> {offset:X8}" );

                Camera.Add( new CameraEntry( Reader, offset ) );
            }
            // ============ TABLE 4 ===============
            for( var i = 0; i < table4_Count; i++ ) {
                var offset = Reader.ReadInt32();
                if( offset == 0 ) continue;
                PluginLog.Log( $"4: {Reader.BaseStream.Position:X8} -> {offset:X8}" );
            }
        }

        private static int RoundUp( int value, int round = 4 ) {
            return round * ( int )Math.Ceiling( value / ( float )round );
        }

        public static void Test() {
            VfxEditor.DataManager.GetFile<ScdFile>( "sound/vfx/se_vfx_common.scd" );
            PluginLog.Log( "---------------------" );
            VfxEditor.DataManager.GetFile<ScdFile>( "sound/vfx/ability/se_vfx_abi_drk_bloodcontract_c.scd" );
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

    // ====== TABLE 0 =========
    public class Table0Entry : ScdEntry {
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

        public Table0Entry( BinaryReader reader ) : base( reader ) { }
        public Table0Entry( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Header = reader.ReadStructure<Table0_Header>();
        }
    }

    // ===== TABLE 2 (MUSIC) ==========
    public class MusicEntry : ScdEntry {
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

        public MusicEntry( BinaryReader reader ) : base( reader ) { }
        public MusicEntry( BinaryReader reader, int offset ) : base( reader, offset ) { }

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

    // ===== TABLE 3 (CAMERA STUFF) ==========
    public class CameraEntry : ScdEntry {
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

        public CameraEntry( BinaryReader reader ) : base( reader ) { }
        public CameraEntry( BinaryReader reader, int offset ) : base( reader, offset ) { }

        protected override void Read( BinaryReader reader ) {
            Data = reader.ReadStructure<Camera_Data>();

            PluginLog.Log( $"TABLE 3: {Data.Size} {Data.Unk1} {Data.Unk2} {Data.Unk3} {Data.Unk4}" );
        }
    }
}

