using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using Penumbra.String.Classes;
using System;
using System.Collections.Generic;
using System.Threading;
using VfxEditor.Structs;

namespace VfxEditor.Interop {
    // https://github.com/xivdev/Penumbra/blob/master/Penumbra/Interop/Hooks/ResourceLoading/TexMdlService.cs

    public unsafe partial class ResourceLoader {
        public static readonly IntPtr CustomFileFlag = new( 0xDEADBEEE );

        private readonly HashSet<ulong> CustomMdlCrc = [];
        private readonly HashSet<ulong> CustomTexCrc = [];

        private readonly ThreadLocal<bool> TexReturnData = new( () => default );

        // ====== LOD ==========

        [Signature( Constants.LodConfigSig )]
        private readonly nint LodConfig = nint.Zero;

        public byte GetLod( TextureResourceHandle* handle ) {
            if( handle->ChangeLod ) {
                var config = *( byte* )LodConfig + 0xE;
                if( config == byte.MaxValue ) return 2;
            }
            return 0;
        }

        // ======= CRC =========

        public delegate IntPtr CheckFileStatePrototype( IntPtr unk1, ulong crc64 );

        public Hook<CheckFileStatePrototype> CheckFileStateHook { get; private set; }

        private nint CheckFileStateDetour( nint ptr, ulong crc64 ) {
            if( CustomMdlCrc.Contains( crc64 ) ) return CustomFileFlag;
            if( CustomTexCrc.Contains( crc64 ) ) {
                TexReturnData.Value = true;
            }
            return CheckFileStateHook.Original( ptr, crc64 );
        }

        public void AddCrc( ResourceType type, FullPath? path ) {
            _ = type switch {
                ResourceType.Mdl when path.HasValue => CustomMdlCrc.Add( path.Value.Crc64 ),
                ResourceType.Tex when path.HasValue => CustomTexCrc.Add( path.Value.Crc64 ),
                _ => false,
            };
        }

        // ====== LOAD TEX ==========

        public delegate byte LoadTexFileLocalDelegate( TextureResourceHandle* handle, int unk1, SeFileDescriptor* unk2, bool unk33 );

        public LoadTexFileLocalDelegate LoadTexFileLocal { get; private set; }

        private delegate byte TexResourceHandleOnLoadPrototype( TextureResourceHandle* handle, SeFileDescriptor* descriptor, byte unk2 );

        [Signature( Constants.TexResourceHandleOnLoadSig, DetourName = nameof( TexOnLoadDetour ) )]
        private readonly Hook<TexResourceHandleOnLoadPrototype> TextureOnLoadHook = null!;

        private byte TexOnLoadDetour( TextureResourceHandle* handle, SeFileDescriptor* descriptor, byte unk2 ) {
            var ret = TextureOnLoadHook.Original( handle, descriptor, unk2 );
            if( !TexReturnData.Value )
                return ret;

            // Function failed on a replaced texture, call local.
            TexReturnData.Value = false;
            return LoadTexFileLocal( handle, GetLod( handle ), descriptor, unk2 != 0 );
        }

        // ========= LOAD MDL ============

        public delegate byte LoadMdlFileLocalDelegate( ResourceHandle* handle, IntPtr unk1, bool unk2 );

        public LoadMdlFileLocalDelegate LoadMdlFileLocal { get; private set; }

        public delegate byte LoadMdlFileExternDelegate( ResourceHandle* handle, IntPtr unk1, bool unk2, IntPtr unk3 );

        public Hook<LoadMdlFileExternDelegate> LoadMdlFileExternHook { get; private set; }

        private byte LoadMdlFileExternDetour( ResourceHandle* resourceHandle, IntPtr unk1, bool unk2, IntPtr ptr )
        => ptr.Equals( CustomFileFlag )
            ? LoadMdlFileLocal.Invoke( resourceHandle, unk1, unk2 )
            : LoadMdlFileExternHook.Original( resourceHandle, unk1, unk2, ptr );
    }
}
