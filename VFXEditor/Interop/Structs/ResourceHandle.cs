using FFXIVClientStructs.FFXIV.Client.System.Resource;
using Penumbra.String;
using Penumbra.String.Classes;
using System;
using System.Runtime.InteropServices;

namespace VfxEditor.Structs {
    [StructLayout( LayoutKind.Explicit )]
    public unsafe struct ResourceHandle {
        [StructLayout( LayoutKind.Explicit )]
        public struct DataIndirection {
            [FieldOffset( 0x00 )]
            public void** VTable;

            [FieldOffset( 0x10 )]
            public byte* DataPtr;

            [FieldOffset( 0x28 )]
            public ulong DataLength;
        }

        public const int SsoSize = 15;

        public byte* FileNamePtr() {
            if( FileNameLength > SsoSize ) {
                return FileNameData;
            }

            fixed( byte** name = &FileNameData ) {
                return ( byte* )name;
            }
        }

        public ByteString FileName()
            => ByteString.FromByteStringUnsafe( FileNamePtr(), FileNameLength, true );

        public bool GamePath( out Utf8GamePath path )
            => Utf8GamePath.FromSpan( new ReadOnlySpan<byte>( FileNamePtr(), FileNameLength ), out path );

        [FieldOffset( 0x00 )]
        public void** VTable;

        [FieldOffset( 0x08 )]
        public ResourceCategory Category;

        [FieldOffset( 0x10 )]
        public uint Id;

        [FieldOffset( 0x28 )]
        public uint FileSize;

        [FieldOffset( 0x2C )]
        public uint FileSize2;

        [FieldOffset( 0x34 )]
        public uint FileSize3;

        [FieldOffset( 0x48 )]
        public byte* FileNameData;

        [FieldOffset( 0x58 )]
        public int FileNameLength;

        [FieldOffset( 0xAC )]
        public uint RefCount;
    }
}