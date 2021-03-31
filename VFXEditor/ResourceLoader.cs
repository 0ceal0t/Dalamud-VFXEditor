using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Plugin;
using VFXEditor.Structs;
using VFXEditor.Util;
using FileMode = VFXEditor.Structs.FileMode;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using System.Threading.Tasks;
using System.Threading;

namespace VFXEditor
{
    public class ResourceLoader : IDisposable
    {
        public Plugin Plugin { get; set; }
        public bool IsEnabled { get; set; }
        public Crc32 Crc32 { get; }

        // ===== FILES =========
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate byte ReadFilePrototype( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync );
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate byte ReadSqpackPrototype( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync );
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate void* GetResourceSyncPrototype( IntPtr pFileManager, uint* pCategoryId, char* pResourceType,
            uint* pResourceHash, char* pPath, void* pUnknown );
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate void* GetResourceAsyncPrototype( IntPtr pFileManager, uint* pCategoryId, char* pResourceType,
            uint* pResourceHash, char* pPath, void* pUnknown, bool isUnknown );
        // ====== FILES HOOKS ========
        public IHook< GetResourceSyncPrototype > GetResourceSyncHook { get; private set; }
        public IHook< GetResourceAsyncPrototype > GetResourceAsyncHook { get; private set; }
        public IHook< ReadSqpackPrototype > ReadSqpackHook { get; private set; }
        public ReadFilePrototype ReadFile { get; private set; }

        //====== STATIC ===========
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public delegate IntPtr VfxCreateDelegate( string path, string pool );
        public VfxCreateDelegate VfxCreate;
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public delegate IntPtr VfxRunDelegate( IntPtr vfx, float a1, uint a2 );
        public VfxRunDelegate VfxRun;
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public delegate IntPtr VfxRemoveDelegate( IntPtr vfx );
        public VfxRemoveDelegate VfxRemove;
        // ======= STATIC HOOKS ========
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr VfxCreateHook( char* path, char* pool );
        public IHook<VfxCreateHook> StaticVfxNewHook { get; private set; }
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr VfxRemoveHook( IntPtr vfx );
        public IHook<VfxRemoveHook> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public delegate IntPtr StatusAddDelegate( string a1, IntPtr a2, IntPtr a3, float a4, char a5, UInt16 a6, char a7 );
        public StatusAddDelegate StatusAdd;
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public delegate IntPtr StatusRemoveDelegate( IntPtr vfx, char a2 );
        public StatusRemoveDelegate StatusRemove;
        // ======== ACTOR HOOKS =============
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr VfxStatusAddHook( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, UInt16 a6, char a7 );
        public IHook<VfxStatusAddHook> ActorVfxNewHook { get; private set; }
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr VfxStatusRemoveHook( IntPtr vfx, char a2 );
        public IHook<VfxStatusRemoveHook> ActorVfxRemoveHook { get; private set; }

        // ======== WEAPON HOOKS ==============
        //[Function( CallingConventions.Microsoft )]
        //public unsafe delegate IntPtr VfxWeaponAddHook( IntPtr a1, Int32 a2, IntPtr a3, char a4, char a5, char a6, char a7 );
        //public IHook<VfxWeaponAddHook> WeaponAddHook { get; private set; }

#if !DEBUG
        public bool EnableHooks = true;
#else
        public bool EnableHooks = false;
#endif


        public ResourceLoader( Plugin plugin ) {
            Plugin = plugin;
            Crc32 = new Crc32();
        }

        // https://github.com/xivdev/Penumbra/blob/master/Penumbra/ResourceLoader.cs
        public unsafe void Init()
        {
            var scanner = Plugin.PluginInterface.TargetModuleScanner;

            var readFileAddress = scanner.ScanText( "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3 BA 05" );
            var readSqpackAddress = scanner.ScanText( "E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3" );
            var getResourceSyncAddress = scanner.ScanText( "E8 ?? ?? 00 00 48 8D 4F ?? 48 89 87 ?? ?? 00 00" );
            var getResourceAsyncAddress = scanner.ScanText( "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00" );
            if( EnableHooks ) {
                ReadSqpackHook = new Hook<ReadSqpackPrototype>( ReadSqpackHandler, ( long )readSqpackAddress );
                GetResourceSyncHook = new Hook<GetResourceSyncPrototype>( GetResourceSyncHandler, ( long )getResourceSyncAddress );
                GetResourceAsyncHook = new Hook<GetResourceAsyncPrototype>( GetResourceAsyncHandler, ( long )getResourceAsyncAddress );
                ReadFile = Marshal.GetDelegateForFunctionPointer<ReadFilePrototype>( readFileAddress );
            }

            var vfxCreateAddress = scanner.ScanText( "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08" );
            var vfxRunAddress = scanner.ScanText( "E8 ?? ?? ?? ?? 0F 28 B4 24 ?? ?? ?? ?? 48 8B 8C 24 ?? ?? ?? ?? 48 33 CC E8 ?? ?? ?? ?? 48 8B 9C 24 ?? ?? ?? ?? 48 81 C4 ?? ?? ?? ?? 5F" );
            var vfxRemoveAddress = scanner.ScanText( "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9" );
            var statusAddAddr = scanner.ScanText( "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8" );
            var statusRemoveAddr2 = scanner.ScanText( "0F 11 48 10 48 8D 05" ) + 7;
            var statusRemove2 = Marshal.ReadIntPtr( statusRemoveAddr2 + Marshal.ReadInt32( statusRemoveAddr2 ) + 4 );

            StatusAdd = Marshal.GetDelegateForFunctionPointer<StatusAddDelegate>( statusAddAddr );
            StatusRemove = Marshal.GetDelegateForFunctionPointer<StatusRemoveDelegate>( statusRemove2 );
            VfxRemove = Marshal.GetDelegateForFunctionPointer<VfxRemoveDelegate>( vfxRemoveAddress );
            VfxRun = Marshal.GetDelegateForFunctionPointer<VfxRunDelegate>( vfxRunAddress );
            VfxCreate = Marshal.GetDelegateForFunctionPointer<VfxCreateDelegate>( vfxCreateAddress );
            if( EnableHooks ) {
                StaticVfxNewHook = new Hook<VfxCreateHook>( StaticVfxNewHandler, ( long )vfxCreateAddress );
                StaticVfxRemoveHook = new Hook<VfxRemoveHook>( StaticVfxRemoveHandler, ( long )vfxRemoveAddress );
                ActorVfxNewHook = new Hook<VfxStatusAddHook>( ActorVfxNewHandler, ( long )statusAddAddr );
                ActorVfxRemoveHook = new Hook<VfxStatusRemoveHook>( ActorVfxRemoveHandler, ( long )statusRemove2 );
            }
        }
        // ============
        private unsafe IntPtr StaticVfxNewHandler( char* path, char* pool ) {
            var v = StaticVfxNewHook.OriginalFunction( path, pool );
            var p1 = Marshal.PtrToStringAnsi( new IntPtr( path ) );
            Plugin.Tracker.AddStatic( v, p1 );
            return v;
        }
        private unsafe IntPtr StaticVfxRemoveHandler( IntPtr vfx ) {
            if( Plugin.MainUI?.SpawnVfx != null && vfx == Plugin.MainUI.SpawnVfx.Vfx ) {
                Plugin.MainUI.SpawnVfx = null;
            }
            Plugin.Tracker.RemoveStatic( vfx );
            return StaticVfxRemoveHook.OriginalFunction( vfx );
        }
        // ============
        private unsafe IntPtr ActorVfxNewHandler( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, UInt16 a6, char a7 ) {
            var v = ActorVfxNewHook.OriginalFunction( a1, a2, a3, a4, a5, a6, a7 );
            var p1 = Marshal.PtrToStringAnsi( new IntPtr( a1 ) );
            Plugin.Tracker.AddActor( a2, v, p1 );
            return v;
        }
        private unsafe IntPtr ActorVfxRemoveHandler( IntPtr vfx, char a2 ) {
            if( Plugin.MainUI?.SpawnVfx != null && vfx == Plugin.MainUI.SpawnVfx.Vfx ) {
                Plugin.MainUI.SpawnVfx = null;
            }
            Plugin.Tracker.RemoveActor( vfx );
            return ActorVfxRemoveHook.OriginalFunction( vfx, a2 );
        }

        public void Enable() {
            if( IsEnabled )
                return;
            if( EnableHooks ) {
                ReadSqpackHook.Activate();
                GetResourceSyncHook.Activate();
                GetResourceAsyncHook.Activate();

                StaticVfxNewHook.Activate();
                StaticVfxRemoveHook.Activate();
                ActorVfxNewHook.Activate();
                ActorVfxRemoveHook.Activate();

                // ==============
                ReadSqpackHook.Enable();
                GetResourceSyncHook.Enable();
                GetResourceAsyncHook.Enable();

                StaticVfxNewHook.Enable();
                StaticVfxRemoveHook.Enable();
                ActorVfxNewHook.Enable();
                ActorVfxRemoveHook.Enable();
            }
            IsEnabled = true;
        }

        public void Disable() {
            if( !IsEnabled )
                return;
            if( EnableHooks ) {
                ReadSqpackHook.Disable();
                GetResourceSyncHook.Disable();
                GetResourceAsyncHook.Disable();

                StaticVfxNewHook.Disable();
                StaticVfxRemoveHook.Disable();
                ActorVfxNewHook.Disable();
                ActorVfxRemoveHook.Disable();
            }
            IsEnabled = false;
        }

        // https://github.com/imchillin/Anamnesis/blob/0ba09fcd7fb1ec1ed13b22ab9e5b2cea6926f113/Anamnesis/Core/Memory/AddressService.cs
        // https://github.com/imchillin/CMTool/blob/a1af42ceab86700d4d1b21b5ba61079ad79fd2f2/ConceptMatrix/OffsetSettings.json#L69
        // https://git.ava.dev/ava/OopsAllLalafells/src/branch/master/Plugin.cs#L145
        public unsafe void ReRender() {
            var player = Plugin.PluginInterface.ClientState.LocalPlayer;
            var charBaseAddr = player.Address;

            Task.Run( () =>
            {
                var entityOffset = charBaseAddr + Dalamud.Game.ClientState.Structs.ActorOffsets.ObjectKind;
                var renderOffset = charBaseAddr + 0x104;

                Marshal.WriteByte( entityOffset, 0x02 );
                Marshal.WriteByte( renderOffset, 0x02 );
                Thread.Sleep( 100 );
                Marshal.WriteByte( renderOffset, 0x00 );
                Thread.Sleep( 100 );
                Marshal.WriteByte( entityOffset, 0x01 );
            } );
        }

        private unsafe void* GetResourceSyncHandler(
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            uint* pResourceHash,
            char* pPath,
            void* pUnknown
        ) => GetResourceHandler( true, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, false );

        private unsafe void* GetResourceAsyncHandler(
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            uint* pResourceHash,
            char* pPath,
            void* pUnknown,
            bool isUnknown
        ) => GetResourceHandler( false, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );

        private unsafe void* CallOriginalHandler(
            bool isSync,
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            uint* pResourceHash,
            char* pPath,
            void* pUnknown,
            bool isUnknown
        ) => isSync
            ? GetResourceSyncHook.OriginalFunction( pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown )
            : GetResourceAsyncHook.OriginalFunction( pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );

        private unsafe void* GetResourceHandler(
            bool isSync, IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            uint* pResourceHash,
            char* pPath,
            void* pUnknown,
            bool isUnknown
        ) {
            var gameFsPath = Marshal.PtrToStringAnsi( new IntPtr( pPath ) );
            if( Configuration.Config?.LogAllFiles == true ) {
                PluginLog.Log( "[GetResourceHandler] {0}", gameFsPath );
            }
            // ============ REPLACE THE FILE ============
            FileInfo replaceFile = null;
            if(Plugin.Doc.GetLocalPath(gameFsPath, out var vfxFile ) ) {
                replaceFile = vfxFile;
            }
            else if(Plugin.Manager.TexManager.GetLocalPath(gameFsPath, out var texFile ) ) {
                replaceFile = texFile;
            }

            var fsPath = replaceFile?.FullName;

            if( fsPath == null || fsPath.Length >= 260 ) {
                return CallOriginalHandler( isSync, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );
            }
            var cleanPath = fsPath.Replace( '\\', '/' );
            var path = Encoding.ASCII.GetBytes( cleanPath );
            var bPath = stackalloc byte[path.Length + 1];
            Marshal.Copy( path, 0, new IntPtr( bPath ), path.Length );
            pPath = ( char* )bPath;
            Crc32.Init();
            Crc32.Update( path );
            *pResourceHash = Crc32.Checksum;
            return CallOriginalHandler( isSync, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );
        }


        private unsafe byte ReadSqpackHandler( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync ) {
            var gameFsPath = Marshal.PtrToStringAnsi( new IntPtr( pFileDesc->ResourceHandle->FileName ) );
            var isRooted = Path.IsPathRooted( gameFsPath );
            if( gameFsPath == null || gameFsPath.Length >= 260 || !isRooted ) {
                return ReadSqpackHook.OriginalFunction( pFileHandler, pFileDesc, priority, isSync );
            }
            pFileDesc->FileMode = FileMode.LoadUnpackedResource;

            // note: must be utf16
            var utfPath = Encoding.Unicode.GetBytes( gameFsPath );
            Marshal.Copy( utfPath, 0, new IntPtr( &pFileDesc->UtfFileName ), utfPath.Length );
            var fd = stackalloc byte[0x20 + utfPath.Length + 0x16];
            Marshal.Copy( utfPath, 0, new IntPtr( fd + 0x21 ), utfPath.Length );
            pFileDesc->FileDescriptor = fd;
            return ReadFile( pFileHandler, pFileDesc, priority, isSync );
        }

        public void Dispose() {
            if( IsEnabled )
                Disable();
        }
    }
}