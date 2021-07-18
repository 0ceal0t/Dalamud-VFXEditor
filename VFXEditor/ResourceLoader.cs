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
using VFXEditor.Structs.Vfx;

namespace VFXEditor
{
    public class ResourceLoader : IDisposable
    {
        public Plugin Plugin { get; set; }
        public bool IsEnabled { get; set; }
        public Crc32 Crc32 { get; }

        // ====== REDRAW =======
        private enum RedrawState {
            None,
            Start,
            Invisible,
            Visible
        }
        private RedrawState CurrentRedrawState = RedrawState.None;
        IntPtr RenderPtr;

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
        public unsafe delegate VfxStruct* StaticVfxCreateDelegate( string path, string pool );
        public StaticVfxCreateDelegate StaticVfxCreate;
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr StaticVfxRunDelegate( VfxStruct* vfx, float a1, uint a2 );
        public StaticVfxRunDelegate StaticVfxRun;
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr StaticVfxRemoveDelegate( VfxStruct* vfx );
        public StaticVfxRemoveDelegate StaticVfxRemove;
        // ======= STATIC HOOKS ========
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate VfxStruct* StaticVfxCreateDelegate2( char* path, char* pool );
        public IHook<StaticVfxCreateDelegate2> StaticVfxCreateHook { get; private set; }
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr StaticVfxRemoveDelegate2( VfxStruct* vfx );
        public IHook<StaticVfxRemoveDelegate2> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate VfxStruct* ActorVfxCreateDelegate( string a1, IntPtr a2, IntPtr a3, float a4, char a5, UInt16 a6, char a7 );
        public ActorVfxCreateDelegate ActorVfxCreate;
        [UnmanagedFunctionPointer( CallingConvention.Cdecl, CharSet = CharSet.Ansi )]
        public unsafe delegate IntPtr ActorVfxRemoveDelegate( VfxStruct* vfx, char a2 );
        public ActorVfxRemoveDelegate ActorVfxRemove;
        // ======== ACTOR HOOKS =============
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate VfxStruct* ActorVfxCreateDelegate2( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, UInt16 a6, char a7 );
        public IHook<ActorVfxCreateDelegate2> ActorVfxCreateHook { get; private set; }
        [Function( CallingConventions.Microsoft )]
        public unsafe delegate IntPtr ActorVfxRemoveDelegate2( VfxStruct* vfx, char a2 );
        public IHook<ActorVfxRemoveDelegate2> ActorVfxRemoveHook { get; private set; }

        // ========= MISC ==============
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        internal delegate IntPtr GetMatrixSingletonDelegate();
        internal GetMatrixSingletonDelegate GetMatrixSingleton;


        public ResourceLoader( Plugin plugin ) {
            Plugin = plugin;
            Crc32 = new Crc32();
        }

        // https://github.com/xivdev/Penumbra/blob/master/Penumbra/ResourceLoader.cs
        public unsafe void Init() {
            var scanner = Plugin.PluginInterface.TargetModuleScanner;

            var readFileAddress = scanner.ScanText( "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3 BA 05" );
            var readSqpackAddress = scanner.ScanText( "E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3" );
            var getResourceSyncAddress = scanner.ScanText( "E8 ?? ?? 00 00 48 8D 8F ?? ?? 00 00 48 89 87 ?? ?? 00 00" );
            var getResourceAsyncAddress = scanner.ScanText( "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00" );

            ReadSqpackHook = new Hook<ReadSqpackPrototype>( ReadSqpackHandler, ( long )readSqpackAddress );
            GetResourceSyncHook = new Hook<GetResourceSyncPrototype>( GetResourceSyncHandler, ( long )getResourceSyncAddress );
            GetResourceAsyncHook = new Hook<GetResourceAsyncPrototype>( GetResourceAsyncHandler, ( long )getResourceAsyncAddress );
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

            StaticVfxCreateHook = new Hook<StaticVfxCreateDelegate2>( StaticVfxNewHandler, ( long )staticVfxCreateAddress );
            StaticVfxRemoveHook = new Hook<StaticVfxRemoveDelegate2>( StaticVfxRemoveHandler, ( long )staticVfxRemoveAddress );

            ActorVfxCreateHook = new Hook<ActorVfxCreateDelegate2>( ActorVfxNewHandler, ( long )actorVfxCreateAddress );
            ActorVfxRemoveHook = new Hook<ActorVfxRemoveDelegate2>( ActorVfxRemoveHandler, ( long )actorVfxRemoveAddress );

            var matrixAddr = scanner.ScanText( "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4c 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??" );
            GetMatrixSingleton = Marshal.GetDelegateForFunctionPointer<GetMatrixSingletonDelegate>( matrixAddr );
        }

        private unsafe VfxStruct* StaticVfxNewHandler( char* path, char* pool ) {
            var vfx = StaticVfxCreateHook.OriginalFunction( path, pool );
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( path ), Encoding.ASCII );
            Plugin.Tracker?.AddStatic( vfx, vfxPath );
            return vfx;
        }

        private unsafe IntPtr StaticVfxRemoveHandler( VfxStruct* vfx ) {
            if( Plugin.SpawnVfx != null && vfx == Plugin.SpawnVfx.Vfx ) {
                Plugin.SpawnVfx = null;
            }
            Plugin.Tracker?.RemoveStatic( vfx );
            return StaticVfxRemoveHook.OriginalFunction( vfx );
        }

        private unsafe VfxStruct* ActorVfxNewHandler( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 ) {
            var vfx = ActorVfxCreateHook.OriginalFunction( a1, a2, a3, a4, a5, a6, a7 );
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( a1 ), Encoding.ASCII );
            Plugin.Tracker?.AddActor( a2, vfx, vfxPath );
            return vfx;
        }

        private unsafe IntPtr ActorVfxRemoveHandler( VfxStruct* vfx, char a2 ) {
            if( Plugin.SpawnVfx != null && vfx == Plugin.SpawnVfx.Vfx ) {
                Plugin.SpawnVfx = null;
            }
            Plugin.Tracker?.RemoveActor( vfx );
            return ActorVfxRemoveHook.OriginalFunction( vfx, a2 );
        }

        public void Enable() {
            if( IsEnabled )
                return;
            ReadSqpackHook.Activate();
            GetResourceSyncHook.Activate();
            GetResourceAsyncHook.Activate();

            StaticVfxCreateHook.Activate();
            StaticVfxRemoveHook.Activate();
            ActorVfxCreateHook.Activate();
            ActorVfxRemoveHook.Activate();

            // ==============
            ReadSqpackHook.Enable();
            GetResourceSyncHook.Enable();
            GetResourceAsyncHook.Enable();

            StaticVfxCreateHook.Enable();
            StaticVfxRemoveHook.Enable();
            ActorVfxCreateHook.Enable();
            ActorVfxRemoveHook.Enable();
            IsEnabled = true;
        }

        public void Disable() {
            if( !IsEnabled )
                return;
            ReadSqpackHook.Disable();
            GetResourceSyncHook.Disable();
            GetResourceAsyncHook.Disable();

            StaticVfxCreateHook.Disable();
            StaticVfxRemoveHook.Disable();
            ActorVfxCreateHook.Disable();
            ActorVfxRemoveHook.Disable();
            IsEnabled = false;
        }

        public void ReRender() {
            var player = Plugin.PluginInterface.ClientState.LocalPlayer;
            RenderPtr = player.Address + 0x104;

            if( CurrentRedrawState != RedrawState.None ) return;
            CurrentRedrawState = RedrawState.Start;
            Plugin.PluginInterface.Framework.OnUpdateEvent += OnUpdateEvent;
        }

        private unsafe void OnUpdateEvent(object framework) {
            switch(CurrentRedrawState) {
                case RedrawState.Start:
                    *( int* )RenderPtr |= 0x00_00_00_02;
                    CurrentRedrawState = RedrawState.Invisible;
                    break;
                case RedrawState.Invisible:
                    *( int* )RenderPtr &= ~0x00_00_00_02;
                    CurrentRedrawState = RedrawState.Visible;
                    break;
                case RedrawState.Visible:
                default:
                    CurrentRedrawState = RedrawState.None;
                    Plugin.PluginInterface.Framework.OnUpdateEvent -= OnUpdateEvent;
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

            if( Plugin.DocManager != null && Plugin.DocManager.GetLocalPath(gameFsPath, out var vfxFile ) ) {
                replaceFile = vfxFile;
                if(Configuration.Config?.LogAllFiles == true) {
                    PluginLog.Log( $"Loaded VFX {gameFsPath} from {replaceFile?.FullName}" );
                }
            }
            else if( Plugin.TexManager != null && Plugin.TexManager.GetLocalReplacePath( gameFsPath, out var texFile ) ) {
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
            var gameFsPath = GetString( pFileDesc->ResourceHandle->File );

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

        public unsafe static string GetString(StdString str) {
            var len = (int) str.Size;
            if( len > 15 ) {
                return Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( str.BufferPtr ), Encoding.ASCII, len );
            }
            return Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( &str.BufferPtr ), Encoding.ASCII, len );
        }
    }
}