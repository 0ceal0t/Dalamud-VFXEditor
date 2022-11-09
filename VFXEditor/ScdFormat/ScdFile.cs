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
            public int OffsetTable2; // offset to list of offsets
            public int OffsetSound;
            public int OffsetTable4;
            public int Padding;
            public int OffsetTable5;
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

            List<int> allPositions = new();
        }

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
}

