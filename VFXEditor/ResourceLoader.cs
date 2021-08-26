using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Logging;
using VFXEditor.Structs;
using VFXEditor.Util;
using VFXEditor.Structs.Vfx;
using VFXEditor.Data.Texture;
using VFXEditor.Data;
using FileMode = VFXEditor.Structs.FileMode;

using Dalamud.Hooking;
using Reloaded.Hooks.Definitions.X64;
using System.Threading;

namespace VFXEditor {
    public class ResourceLoader : IDisposable {
        private Plugin Plugin;
        private bool IsEnabled;
        private Crc32 Crc32;

        // ====== REDRAW =======
        private enum RedrawState {
            None,
            Start,
            Invisible,
            Visible
        }
        private RedrawState CurrentRedrawState = RedrawState.None;
        private int WaitFrames = 0;

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
        public Hook<GetResourceSyncPrototype> GetResourceSyncHook { get; private set; }
        public Hook<GetResourceAsyncPrototype> GetResourceAsyncHook { get; private set; }
        public Hook<ReadSqpackPrototype> ReadSqpackHook { get; private set; }
        public ReadFilePrototype ReadFile { get; private set; }

        //====== STATIC ===========
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr StaticVfxCreateDelegate( string path, string pool );
        public StaticVfxCreateDelegate StaticVfxCreate;

        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr StaticVfxRunDelegate( IntPtr vfx, float a1, uint a2 );
        public StaticVfxRunDelegate StaticVfxRun;

        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr StaticVfxRemoveDelegate( IntPtr vfx );
        public StaticVfxRemoveDelegate StaticVfxRemove;

        // ======= STATIC HOOKS ========
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr StaticVfxCreateDelegate2( char* path, char* pool );
        public Hook<StaticVfxCreateDelegate2> StaticVfxCreateHook { get; private set; }

        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr StaticVfxRemoveDelegate2( IntPtr vfx );
        public Hook<StaticVfxRemoveDelegate2> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr ActorVfxCreateDelegate( string a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );
        public ActorVfxCreateDelegate ActorVfxCreate;

        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr ActorVfxRemoveDelegate( IntPtr vfx, char a2 );
        public ActorVfxRemoveDelegate ActorVfxRemove;

        // ======== ACTOR HOOKS =============
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr ActorVfxCreateDelegate2( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );
        public Hook<ActorVfxCreateDelegate2> ActorVfxCreateHook { get; private set; }

        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr ActorVfxRemoveDelegate2( IntPtr vfx, char a2 );
        public Hook<ActorVfxRemoveDelegate2> ActorVfxRemoveHook { get; private set; }

        // ========= MISC ==============
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        internal delegate IntPtr GetMatrixSingletonDelegate();
        internal GetMatrixSingletonDelegate GetMatrixSingleton;


        public ResourceLoader(Plugin plugin) {
            Crc32 = new Crc32();
            Plugin = plugin;
        }

        public unsafe void Init() {
            var scanner = Plugin.SigScanner;

            var readFileAddress = scanner.ScanText( "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3 BA 05" );
            var readSqpackAddress = scanner.ScanText( "E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3" );
            var getResourceSyncAddress = scanner.ScanText( "E8 ?? ?? 00 00 48 8D 8F ?? ?? 00 00 48 89 87 ?? ?? 00 00" );
            var getResourceAsyncAddress = scanner.ScanText( "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00" );

            ReadSqpackHook = new Hook<ReadSqpackPrototype>( readSqpackAddress, ReadSqpackHandler );
            GetResourceSyncHook = new Hook<GetResourceSyncPrototype>( getResourceSyncAddress, GetResourceSyncHandler );
            GetResourceAsyncHook = new Hook<GetResourceAsyncPrototype>( getResourceAsyncAddress, GetResourceAsyncHandler );

            ReadFile = Marshal.GetDelegateForFunctionPointer<ReadFilePrototype>( readFileAddress );

            var staticVfxCreateAddress = scanner.ScanText( "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08" );
            var staticVfxRunAddress = scanner.ScanText( "E8 ?? ?? ?? ?? 0F 28 B4 24 ?? ?? ?? ?? 48 8B 8C 24 ?? ?? ?? ?? 48 33 CC E8 ?? ?? ?? ?? 48 8B 9C 24 ?? ?? ?? ?? 48 81 C4 ?? ?? ?? ?? 5F" );
            var staticVfxRemoveAddress = scanner.ScanText( "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9" );

            var actorVfxCreateAddress = scanner.ScanText( "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8" );
            var actorVfxRemoveAddress_1 = scanner.ScanText( "0F 11 48 10 48 8D 05" ) + 7;
            var actorVfxRemoveAddress = Marshal.ReadIntPtr( actorVfxRemoveAddress_1 + Marshal.ReadInt32( actorVfxRemoveAddress_1 ) + 4 );

            ActorVfxCreate = Marshal.GetDelegateForFunctionPointer<ActorVfxCreateDelegate>( actorVfxCreateAddress );
            ActorVfxRemove = Marshal.GetDelegateForFunctionPointer<ActorVfxRemoveDelegate>( actorVfxRemoveAddress );

            StaticVfxRemove = Marshal.GetDelegateForFunctionPointer<StaticVfxRemoveDelegate>( staticVfxRemoveAddress );
            StaticVfxRun = Marshal.GetDelegateForFunctionPointer<StaticVfxRunDelegate>( staticVfxRunAddress );
            StaticVfxCreate = Marshal.GetDelegateForFunctionPointer<StaticVfxCreateDelegate>( staticVfxCreateAddress );

            StaticVfxCreateHook = new Hook<StaticVfxCreateDelegate2>( staticVfxCreateAddress, StaticVfxNewHandler );
            StaticVfxRemoveHook = new Hook<StaticVfxRemoveDelegate2>( staticVfxRemoveAddress, StaticVfxRemoveHandler );

            ActorVfxCreateHook = new Hook<ActorVfxCreateDelegate2>( actorVfxCreateAddress, ActorVfxNewHandler );
            ActorVfxRemoveHook = new Hook<ActorVfxRemoveDelegate2>( actorVfxRemoveAddress, ActorVfxRemoveHandler );

            var matrixAddr = scanner.ScanText( "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4c 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??" );
            GetMatrixSingleton = Marshal.GetDelegateForFunctionPointer<GetMatrixSingletonDelegate>( matrixAddr );
        }

        private unsafe IntPtr StaticVfxNewHandler( char* path, char* pool ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( path ), Encoding.ASCII, 256 );
            var vfx = StaticVfxCreateHook.Original( path, pool );
            Plugin.Tracker?.AddStatic( ( VfxStruct* ) vfx, vfxPath );
            return vfx;
        }

        private unsafe IntPtr StaticVfxRemoveHandler( IntPtr vfx ) {
            if( Plugin.SpawnVfx != null && vfx == (IntPtr) Plugin.SpawnVfx.Vfx ) {
                Plugin.ClearSpawnVfx();
            }
            Plugin.Tracker?.RemoveStatic( ( VfxStruct* ) vfx );
            return StaticVfxRemoveHook.Original( vfx );
        }

        private unsafe IntPtr ActorVfxNewHandler( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( a1 ), Encoding.ASCII, 256 );
            var vfx = ActorVfxCreateHook.Original( a1, a2, a3, a4, a5, a6, a7 );
            Plugin.Tracker?.AddActor( ( VfxStruct* ) vfx, vfxPath );
            return vfx;
        }

        private unsafe IntPtr ActorVfxRemoveHandler( IntPtr vfx, char a2 ) {
            if( Plugin.SpawnVfx != null && vfx == (IntPtr) Plugin.SpawnVfx.Vfx ) {
                Plugin.ClearSpawnVfx();
            }
            Plugin.Tracker?.RemoveActor( (VfxStruct*) vfx );
            return ActorVfxRemoveHook.Original( vfx, a2 );
        }

        public void Enable() {
            if( IsEnabled ) return;

            ReadSqpackHook.Enable();
            GetResourceSyncHook.Enable();
            GetResourceAsyncHook.Enable();

            StaticVfxCreateHook.Enable();
            StaticVfxRemoveHook.Enable();
            ActorVfxCreateHook.Enable();
            ActorVfxRemoveHook.Enable();

            IsEnabled = true;
        }

        public void Dispose() {
            if( IsEnabled ) Disable();
        }

        public void Disable() {
            if( !IsEnabled ) return;
            ReadSqpackHook.Disable();
            GetResourceSyncHook.Disable();
            GetResourceAsyncHook.Disable();
            StaticVfxCreateHook.Disable();
            StaticVfxRemoveHook.Disable();
            ActorVfxCreateHook.Disable();
            ActorVfxRemoveHook.Disable();

            Thread.Sleep( 500 );

            ReadSqpackHook.Dispose();
            GetResourceSyncHook.Dispose();
            GetResourceAsyncHook.Dispose();
            StaticVfxCreateHook.Dispose();
            StaticVfxRemoveHook.Dispose();
            ActorVfxCreateHook.Dispose();
            ActorVfxRemoveHook.Dispose();

            ReadSqpackHook = null;
            GetResourceSyncHook = null;
            GetResourceAsyncHook = null;
            StaticVfxCreateHook = null;
            StaticVfxRemoveHook = null;
            ActorVfxCreateHook = null;
            ActorVfxRemoveHook = null;

            IsEnabled = false;
            Plugin = null;
        }

        public void ReRender() {
            if( CurrentRedrawState != RedrawState.None ) return;
            CurrentRedrawState = RedrawState.Start;
            Plugin.Framework.Update += OnUpdateEvent;
        }

        private unsafe void OnUpdateEvent( object framework ) {
            var player = Plugin.ClientState.LocalPlayer;
            var renderPtr = player.Address + 0x104;

            switch( CurrentRedrawState ) {
                case RedrawState.Start:
                    *( int* )renderPtr |= 0x00_00_00_02;
                    CurrentRedrawState = RedrawState.Invisible;
                    WaitFrames = 15;
                    break;
                case RedrawState.Invisible:
                    if(WaitFrames == 0) {
                        *( int* )renderPtr &= ~0x00_00_00_02;
                        CurrentRedrawState = RedrawState.Visible;
                    }
                    else {
                        WaitFrames--;
                    }
                    break;
                case RedrawState.Visible:
                default:
                    CurrentRedrawState = RedrawState.None;
                    Plugin.Framework.Update -= OnUpdateEvent;
                    break;
            }
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
            ? GetResourceSyncHook.Original( pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown )
            : GetResourceAsyncHook.Original( pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );

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

            if( Configuration.Config?.LogAllFiles == true ) PluginLog.Log( "[GetResourceHandler] {0}", gameFsPath );
            FileInfo replaceFile = null;

            if( DocumentManager.Manager?.GetLocalPath( gameFsPath, out var vfxFile ) == true ) replaceFile = vfxFile;
            else if( TextureManager.Manager?.GetLocalReplacePath( gameFsPath, out var texFile ) == true ) replaceFile = texFile;

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
            var gameFsPath = GetString( pFileDesc->ResourceHandle->File );

            var isRooted = Path.IsPathRooted( gameFsPath );
            if( gameFsPath == null || gameFsPath.Length >= 260 || !isRooted ) {
                return ReadSqpackHook.Original( pFileHandler, pFileDesc, priority, isSync );
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

        private static unsafe string GetString( StdString str ) {
            var len = ( int )str.Size;
            if( len > 15 ) {
                return Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( str.BufferPtr ), Encoding.ASCII, len );
            }
            return Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( &str.BufferPtr ), Encoding.ASCII, len );
        }
    }
}