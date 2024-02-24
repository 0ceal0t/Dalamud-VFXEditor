using Dalamud.Hooking;
using Penumbra.String.Classes;
using System;
using System.Collections.Generic;
using VfxEditor.Structs;

namespace VfxEditor.Interop {
    public unsafe partial class ResourceLoader {
        public static readonly IntPtr CustomFileFlag = new( 0xDEADBEEE );
        private readonly HashSet<ulong> CustomCrc = new();

        // ======= CRC =========

        public delegate IntPtr CheckFileStateDelegate( IntPtr unk1, ulong crc64 );

        public Hook<CheckFileStateDelegate> CheckFileStateHook { get; private set; }

        // ====== LOAD TEX ==========

        public delegate byte LoadTexFileLocalDelegate( ResourceHandle* handle, int unk1, IntPtr unk2, bool unk3 );

        public LoadTexFileLocalDelegate LoadTexFileLocal { get; private set; }

        public delegate byte LoadTexFileExternDelegate( ResourceHandle* handle, int unk1, IntPtr unk2, bool unk3, IntPtr unk4 );

        public Hook<LoadTexFileExternDelegate> LoadTexFileExternHook { get; private set; }

        private byte LoadTexFileExternDetour( ResourceHandle* resourceHandle, int unk1, IntPtr unk2, bool unk3, IntPtr ptr )
            => ptr.Equals( CustomFileFlag )
                ? LoadTexFileLocal.Invoke( resourceHandle, unk1, unk2, unk3 )
                : LoadTexFileExternHook.Original( resourceHandle, unk1, unk2, unk3, ptr );

        // ========= LOAD MDL ============

        public delegate byte LoadMdlFileLocalDelegate( ResourceHandle* handle, IntPtr unk1, bool unk2 );

        public LoadMdlFileLocalDelegate LoadMdlFileLocal { get; private set; }

        public delegate byte LoadMdlFileExternDelegate( ResourceHandle* handle, IntPtr unk1, bool unk2, IntPtr unk3 );

        public Hook<LoadMdlFileExternDelegate> LoadMdlFileExternHook { get; private set; }

        private byte LoadMdlFileExternDetour( ResourceHandle* resourceHandle, IntPtr unk1, bool unk2, IntPtr ptr )
        => ptr.Equals( CustomFileFlag )
            ? LoadMdlFileLocal.Invoke( resourceHandle, unk1, unk2 )
            : LoadMdlFileExternHook.Original( resourceHandle, unk1, unk2, ptr );

        // ============================

        private IntPtr CheckFileStateDetour( IntPtr ptr, ulong crc64 )
            => CustomCrc.Contains( crc64 ) ? CustomFileFlag : CheckFileStateHook.Original( ptr, crc64 );

        private void AddCrc( ResourceType resourceType, FullPath? path ) {
            if( path.HasValue && ( resourceType is ResourceType.Tex || resourceType is ResourceType.Mdl ) ) {
                CustomCrc.Add( path.Value.Crc64 );
            }
        }
    }
}
