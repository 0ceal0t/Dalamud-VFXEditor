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

        // Delegate prototypes
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

        [Function( CallingConventions.Microsoft )]
        public unsafe delegate void* LoadPlayerResourcesPrototype( IntPtr pResourceManager );

        [Function( CallingConventions.Microsoft )]
        public unsafe delegate void* UnloadPlayerResourcesPrototype( IntPtr pResourceManager );

        // Hooks
        public IHook< GetResourceSyncPrototype > GetResourceSyncHook { get; private set; }
        public IHook< GetResourceAsyncPrototype > GetResourceAsyncHook { get; private set; }
        public IHook< ReadSqpackPrototype > ReadSqpackHook { get; private set; }

        // Unmanaged functions
        public ReadFilePrototype ReadFile { get; private set; }


        public LoadPlayerResourcesPrototype LoadPlayerResources { get; private set; }
        public UnloadPlayerResourcesPrototype UnloadPlayerResources { get; private set; }

        // Object addresses
        private IntPtr _playerResourceManagerAddress;
        public IntPtr PlayerResourceManagerPtr => Marshal.ReadIntPtr( _playerResourceManagerAddress );


        public bool LogAllFiles = false;
        public bool EnableHooks = false;


        public ResourceLoader( Plugin plugin )
        {
            Plugin = plugin;
            Crc32 = new Crc32();
        }

        public unsafe void Init()
        {
            if( EnableHooks )
            {
                var scanner = Plugin.PluginInterface.TargetModuleScanner;

                var readFileAddress =
                    scanner.ScanText( "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3 BA 05" );
                var readSqpackAddress =
                    scanner.ScanText( "E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3" );
                var getResourceSyncAddress =
                    scanner.ScanText( "E8 ?? ?? 00 00 48 8D 4F ?? 48 89 87 ?? ?? 00 00" );
                var getResourceAsyncAddress =
                    scanner.ScanText( "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00" );

                ReadSqpackHook = new Hook<ReadSqpackPrototype>( ReadSqpackHandler, ( long )readSqpackAddress );
                GetResourceSyncHook = new Hook<GetResourceSyncPrototype>( GetResourceSyncHandler, ( long )getResourceSyncAddress );
                GetResourceAsyncHook = new Hook<GetResourceAsyncPrototype>( GetResourceAsyncHandler, ( long )getResourceAsyncAddress );

                ReadFile = Marshal.GetDelegateForFunctionPointer<ReadFilePrototype>( readFileAddress );

                /////

                var loadPlayerResourcesAddress =
                    scanner.ScanText(
                        "E8 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? BA ?? ?? ?? ?? 41 B8 ?? ?? ?? ?? 48 8B 48 30 48 8B 01 FF 50 10 48 85 C0 74 0A " );
                var unloadPlayerResourcesAddress =
                    scanner.ScanText( "41 55 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 4C 8B E9 48 83 C1 08" );
                _playerResourceManagerAddress = scanner.GetStaticAddressFromSig( "0F 44 FE 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 05" );

                LoadPlayerResources = Marshal.GetDelegateForFunctionPointer<LoadPlayerResourcesPrototype>( loadPlayerResourcesAddress );
                UnloadPlayerResources = Marshal.GetDelegateForFunctionPointer<UnloadPlayerResourcesPrototype>( unloadPlayerResourcesAddress );
                ReadFile = Marshal.GetDelegateForFunctionPointer<ReadFilePrototype>( readFileAddress );
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
            bool isSync,
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            uint* pResourceHash,
            char* pPath,
            void* pUnknown,
            bool isUnknown
        )
        {
            var gameFsPath = Marshal.PtrToStringAnsi( new IntPtr( pPath ) );

            if( LogAllFiles )
            {
                PluginLog.Log( "[GetResourceHandler] {0}", gameFsPath );
            }

            // ============ REPLACE THE FILE ============
            FileInfo replaceFile = null;
            if(gameFsPath == Plugin.ReplaceAVFXPath && !(Plugin.ReplaceAVFXPath == ""))
            {
                replaceFile = new FileInfo(Plugin.Manager.TempPath);
            }
            var fsPath = replaceFile?.FullName;

            // path must be < 260 because statically defined array length :(
            if( fsPath == null || fsPath.Length >= 260 )
            {
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
#if DEBUG
            PluginLog.Log( "[GetResourceHandler] resolved {GamePath} to {NewPath}", gameFsPath, fsPath );
#endif
            return CallOriginalHandler( isSync, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );
        }


        private unsafe byte ReadSqpackHandler( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync )
        {
            var gameFsPath = Marshal.PtrToStringAnsi( new IntPtr( pFileDesc->ResourceHandle->FileName ) );

            var isRooted = Path.IsPathRooted( gameFsPath );
            if( gameFsPath == null || gameFsPath.Length >= 260 || !isRooted )
            {
                return ReadSqpackHook.OriginalFunction( pFileHandler, pFileDesc, priority, isSync );
            }

#if DEBUG
            PluginLog.Log( "loading modded file: {GameFsPath}", gameFsPath );
#endif
            pFileDesc->FileMode = FileMode.LoadUnpackedResource;

            // note: must be utf16
            var utfPath = Encoding.Unicode.GetBytes( gameFsPath );
            Marshal.Copy( utfPath, 0, new IntPtr( &pFileDesc->UtfFileName ), utfPath.Length );
            var fd = stackalloc byte[0x20 + utfPath.Length + 0x16];
            Marshal.Copy( utfPath, 0, new IntPtr( fd + 0x21 ), utfPath.Length );
            pFileDesc->FileDescriptor = fd;

            return ReadFile( pFileHandler, pFileDesc, priority, isSync );
        }

        public unsafe void ReloadPlayerResource()
        {
            ReRender();
            //UnloadPlayerResources( PlayerResourceManagerPtr );
            //LoadPlayerResources( PlayerResourceManagerPtr );
        }

        public unsafe void ReRender()
        {
            if( EnableHooks )
            {
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
        }

        public void Enable()
        {
            if( IsEnabled )
                return;
            if( EnableHooks )
            {
                ReadSqpackHook.Activate();
                GetResourceSyncHook.Activate();
                GetResourceAsyncHook.Activate();

                ReadSqpackHook.Enable();
                GetResourceSyncHook.Enable();
                GetResourceAsyncHook.Enable();
            }
            IsEnabled = true;
        }

        public void Disable()
        {
            if( !IsEnabled )
                return;
            if( EnableHooks )
            {
                ReadSqpackHook.Disable();
                GetResourceSyncHook.Disable();
                GetResourceAsyncHook.Disable();
            }
            IsEnabled = false;
        }

        public void Dispose()
        {
            if( IsEnabled )
                Disable();
        }
    }
}