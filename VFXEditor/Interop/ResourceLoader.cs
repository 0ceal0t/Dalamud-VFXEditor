using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using VfxEditor.Structs;
using VfxEditor.Structs.Vfx;
using FileMode = VfxEditor.Structs.FileMode;
using Penumbra.String;
using Penumbra.String.Classes;

namespace VfxEditor.Interop {
    public unsafe class ResourceLoader : IDisposable {
        public bool IsEnabled { get; private set; } = false;

        private const uint INVIS_FLAG = ( 1 << 1 ) | ( 1 << 11 );

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
        public delegate byte ReadFilePrototype( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync );

        public delegate byte ReadSqpackPrototype( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync );

        public delegate void* GetResourceSyncPrototype( IntPtr fileManager, uint* categoryId, char* resourceType,
            int* resourceHash, byte* path, GetResourceParameters* resParams );

        public delegate void* GetResourceAsyncPrototype( IntPtr fileManager, uint* categoryId, char* resourceType,
            int* resourceHash, byte* path, GetResourceParameters* resParams, bool isUnknown );

        // ====== FILES HOOKS ========
        public Hook<GetResourceSyncPrototype> GetResourceSyncHook { get; private set; }
        public Hook<GetResourceAsyncPrototype> GetResourceAsyncHook { get; private set; }
        public Hook<ReadSqpackPrototype> ReadSqpackHook { get; private set; }
        public ReadFilePrototype ReadFile { get; private set; }

        //====== STATIC ===========
        public delegate IntPtr StaticVfxCreateDelegate( string path, string pool );
        public StaticVfxCreateDelegate StaticVfxCreate;

        public delegate IntPtr StaticVfxRunDelegate( IntPtr vfx, float a1, uint a2 );
        public StaticVfxRunDelegate StaticVfxRun;

        public delegate IntPtr StaticVfxRemoveDelegate( IntPtr vfx );
        public StaticVfxRemoveDelegate StaticVfxRemove;

        // ======= STATIC HOOKS ========
        public delegate IntPtr StaticVfxCreateHookDelegate( char* path, char* pool );
        public Hook<StaticVfxCreateHookDelegate> StaticVfxCreateHook { get; private set; }

        public delegate IntPtr StaticVfxRemoveHookDelegate( IntPtr vfx );
        public Hook<StaticVfxRemoveHookDelegate> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        public delegate IntPtr ActorVfxCreateDelegate( string a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );
        public ActorVfxCreateDelegate ActorVfxCreate;

        public delegate IntPtr ActorVfxRemoveDelegate( IntPtr vfx, char a2 );
        public ActorVfxRemoveDelegate ActorVfxRemove;

        // ======== ACTOR HOOKS =============
        public delegate IntPtr ActorVfxCreateHookDelegate( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );
        public Hook<ActorVfxCreateHookDelegate> ActorVfxCreateHook { get; private set; }

        public delegate IntPtr ActorVfxRemoveHookDelegate( IntPtr vfx, char a2 );
        public Hook<ActorVfxRemoveHookDelegate> ActorVfxRemoveHook { get; private set; }

        // ========= MISC ==============
        public delegate IntPtr GetMatrixSingletonDelegate();
        public GetMatrixSingletonDelegate GetMatrixSingleton { get; private set; }

        public delegate IntPtr GetFileManagerDelegate();
        private GetFileManagerDelegate GetFileManager;
        private GetFileManagerDelegate GetFileManagerAlt;

        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        public delegate byte DecRefDelegate( IntPtr resource );
        private DecRefDelegate DecRef;

        [UnmanagedFunctionPointer( CallingConvention.ThisCall )]
        private delegate void* RequestFileDelegate( IntPtr a1, IntPtr a2, IntPtr a3, byte a4 );
        private RequestFileDelegate RequestFile;

        public void Init() {
            var scanner = Plugin.SigScanner;

            var readFileAddress = scanner.ScanText( Constants.ReadFileSig );

            ReadSqpackHook = Hook<ReadSqpackPrototype>.FromAddress( scanner.ScanText( Constants.ReadSqpackSig ), ReadSqpackHandler );
            GetResourceSyncHook = Hook<GetResourceSyncPrototype>.FromAddress( scanner.ScanText( Constants.GetResourceSyncSig ), GetResourceSyncHandler );
            GetResourceAsyncHook = Hook<GetResourceAsyncPrototype>.FromAddress( scanner.ScanText( Constants.GetResourceAsyncSig ), GetResourceAsyncHandler );

            ReadFile = Marshal.GetDelegateForFunctionPointer<ReadFilePrototype>( readFileAddress );

            var staticVfxCreateAddress = scanner.ScanText( Constants.StaticVfxCreateSig );
            var staticVfxRemoveAddress = scanner.ScanText( Constants.StaticVfxRemoveSig );
            var actorVfxCreateAddress = scanner.ScanText( Constants.ActorVfxCreateSig );
            var actorVfxRemoveAddresTemp = scanner.ScanText( Constants.ActorVfxRemoveSig ) + 7;
            var actorVfxRemoveAddress = Marshal.ReadIntPtr( actorVfxRemoveAddresTemp + Marshal.ReadInt32( actorVfxRemoveAddresTemp ) + 4 );

            ActorVfxCreate = Marshal.GetDelegateForFunctionPointer<ActorVfxCreateDelegate>( actorVfxCreateAddress );
            ActorVfxRemove = Marshal.GetDelegateForFunctionPointer<ActorVfxRemoveDelegate>( actorVfxRemoveAddress );
            StaticVfxRemove = Marshal.GetDelegateForFunctionPointer<StaticVfxRemoveDelegate>( staticVfxRemoveAddress );
            StaticVfxRun = Marshal.GetDelegateForFunctionPointer<StaticVfxRunDelegate>( scanner.ScanText( Constants.StaticVfxRunSig ) );
            StaticVfxCreate = Marshal.GetDelegateForFunctionPointer<StaticVfxCreateDelegate>( staticVfxCreateAddress );

            StaticVfxCreateHook = Hook<StaticVfxCreateHookDelegate>.FromAddress( staticVfxCreateAddress, StaticVfxNewHandler );
            StaticVfxRemoveHook = Hook<StaticVfxRemoveHookDelegate>.FromAddress( staticVfxRemoveAddress, StaticVfxRemoveHandler );
            ActorVfxCreateHook = Hook<ActorVfxCreateHookDelegate>.FromAddress( actorVfxCreateAddress, ActorVfxNewHandler );
            ActorVfxRemoveHook = Hook<ActorVfxRemoveHookDelegate>.FromAddress( actorVfxRemoveAddress, ActorVfxRemoveHandler );

            GetMatrixSingleton = Marshal.GetDelegateForFunctionPointer<GetMatrixSingletonDelegate>( scanner.ScanText( Constants.GetMatrixSig ) );
            GetFileManager = Marshal.GetDelegateForFunctionPointer<GetFileManagerDelegate>( scanner.ScanText( Constants.GetFileManagerSig ) );
            GetFileManagerAlt = Marshal.GetDelegateForFunctionPointer<GetFileManagerDelegate>( scanner.ScanText( Constants.GetFileManager2Sig ) );
            DecRef = Marshal.GetDelegateForFunctionPointer<DecRefDelegate>( scanner.ScanText( Constants.DecRefSig ) );
            RequestFile = Marshal.GetDelegateForFunctionPointer<RequestFileDelegate>( scanner.ScanText( Constants.RequestFileSig ) );
        }

        private IntPtr StaticVfxNewHandler( char* path, char* pool ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( path ), Encoding.ASCII, 256 );
            var vfx = StaticVfxCreateHook.Original( path, pool );
            Plugin.VfxTracker?.AddStatic( ( VfxStruct* )vfx, vfxPath );

            if (Plugin.Configuration?.LogVfxDebug == true) PluginLog.Log( $"New Static: {vfxPath} {vfx:X8}" );

            return vfx;
        }

        private IntPtr StaticVfxRemoveHandler( IntPtr vfx ) {
            if( Plugin.SpawnedVfx != null && vfx == ( IntPtr )Plugin.SpawnedVfx.Vfx ) Plugin.ClearSpawn();
            Plugin.VfxTracker?.RemoveStatic( ( VfxStruct* )vfx );
            return StaticVfxRemoveHook.Original( vfx );
        }

        private IntPtr ActorVfxNewHandler( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( a1 ), Encoding.ASCII, 256 );
            var vfx = ActorVfxCreateHook.Original( a1, a2, a3, a4, a5, a6, a7 );
            Plugin.VfxTracker?.AddActor( ( VfxStruct* )vfx, vfxPath );

            if( Plugin.Configuration?.LogVfxDebug == true ) PluginLog.Log( $"New Actor: {vfxPath} {vfx:X8}" );

            return vfx;
        }

        private IntPtr ActorVfxRemoveHandler( IntPtr vfx, char a2 ) {
            if( Plugin.SpawnedVfx != null && vfx == ( IntPtr )Plugin.SpawnedVfx.Vfx ) Plugin.ClearSpawn();
            Plugin.VfxTracker?.RemoveActor( ( VfxStruct* )vfx );
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
        }

        public void ReRender() {
            if( CurrentRedrawState != RedrawState.None ) return;
            CurrentRedrawState = RedrawState.Start;
            Plugin.Framework.Update += OnUpdateEvent;
        }

        private void OnUpdateEvent( object framework ) {
            var player = Plugin.PlayerObject;
            var renderPtr = player.Address + Constants.RenderOffset;

            switch( CurrentRedrawState ) {
                case RedrawState.Start:
                    *( uint* )renderPtr |= INVIS_FLAG;
                    CurrentRedrawState = RedrawState.Invisible;
                    WaitFrames = 15;
                    break;
                case RedrawState.Invisible:
                    if( WaitFrames == 0 ) {
                        *( uint* )renderPtr &= ~INVIS_FLAG;
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

        private void* GetResourceSyncHandler(
            IntPtr fileManager,
            uint* categoryId,
            char* resourceType,
            int* resourceHash,
            byte* path,
            GetResourceParameters* resParams
        ) => GetResourceHandler( true, fileManager, categoryId, resourceType, resourceHash, path, resParams, false );

        private void* GetResourceAsyncHandler(
            IntPtr fileManager,
            uint* categoryId,
            char* resourceType,
            int* resourceHash,
            byte* path,
            GetResourceParameters* resParams,
            bool isUnknown
        ) => GetResourceHandler( false, fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );

        private void* CallOriginalHandler(
            bool isSync,
            IntPtr fileManager,
            uint* categoryId,
            char* resourceType,
            int* resourceHash,
            byte* path,
            GetResourceParameters* resParams,
            bool isUnknown
        ) => isSync
            ? GetResourceSyncHook.Original( fileManager, categoryId, resourceType, resourceHash, path, resParams )
            : GetResourceAsyncHook.Original( fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );

        private void* GetResourceHandler(
            bool isSync, IntPtr fileManager,
            uint* categoryId,
            char* resourceType,
            int* resourceHash,
            byte* path,
            GetResourceParameters* resParams,
            bool isUnknown
        ) {
            if( !Utf8GamePath.FromPointer( path, out var gamePath ) ) {
                return CallOriginalHandler( isSync, fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );
            }

            var gamePathString = gamePath.ToString();

            if( Plugin.Configuration?.LogAllFiles == true ) PluginLog.Log( "[GetResourceHandler] {0}", gamePathString );

            var replacedPath = GetReplacePath( gamePathString, out var localPath ) ? localPath : null;

            if( replacedPath == null || replacedPath.Length >= 260 ) {
                var unreplaced = CallOriginalHandler( isSync, fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );
                if( Plugin.Configuration?.LogDebug == true && DoDebug( gamePathString ) ) PluginLog.Log( "[GetResourceHandler] Original {0} -> {1} -> {2}", gamePathString, replacedPath, new IntPtr( unreplaced ).ToString( "X8" ) );
                return unreplaced;
            }

            var resolvedPath = new FullPath( replacedPath );

            *resourceHash = InteropUtils.ComputeHash( resolvedPath.InternalName, resParams );
            path = resolvedPath.InternalName.Path;

            var replaced = CallOriginalHandler( isSync, fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "[GetResourceHandler] Replace {0} -> {1} -> {2}", gamePathString, replacedPath, new IntPtr( replaced ).ToString( "X8" ) );
            return replaced;
        }

        private byte ReadSqpackHandler( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync ) {
            if( !pFileDesc->ResourceHandle->GamePath( out var originalGamePath ) ) {
                return ReadSqpackHook.Original( pFileHandler, pFileDesc, priority, isSync );
            }

            var originalPath = originalGamePath.ToString();
            var isPenumbra = ProcessPenumbraPath( originalPath, out var gameFsPath );

            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "[ReadSqpackHandler] {0}", gameFsPath );

            var isRooted = Path.IsPathRooted( gameFsPath );

            // looking for refreshed paths, could also be like |default_1|path.avfx
            if( gameFsPath != null && !isRooted ) {
                var replacementPath = GetReplacePath( gameFsPath, out var localPath ) ? localPath : null;
                if( replacementPath != null && Path.IsPathRooted( replacementPath ) && replacementPath.Length < 260 ) {
                    gameFsPath = replacementPath;
                    isRooted = true;
                    isPenumbra = false;
                }
            }

            // call the original if it's a penumbra path that doesn't need replacement as well
            if( gameFsPath == null || gameFsPath.Length >= 260 || !isRooted || isPenumbra ) {
                if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "[ReadSqpackHandler] Calling Original With {0}", originalPath );
                return ReadSqpackHook.Original( pFileHandler, pFileDesc, priority, isSync );
            }

            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "[ReadSqpackHandler] Replaced with {0}", gameFsPath );

            pFileDesc->FileMode = FileMode.LoadUnpackedResource;

            ByteString.FromString( gameFsPath, out var gamePath );

            // note: must be utf16
            var utfPath = Encoding.Unicode.GetBytes( gameFsPath );
            Marshal.Copy( utfPath, 0, new IntPtr( &pFileDesc->Utf16FileName ), utfPath.Length );
            var fd = stackalloc byte[0x20 + utfPath.Length + 0x16];
            Marshal.Copy( utfPath, 0, new IntPtr( fd + 0x21 ), utfPath.Length );
            pFileDesc->FileDescriptor = fd;

            return ReadFile( pFileHandler, pFileDesc, priority, isSync );
        }

        private static bool ProcessPenumbraPath( string path, out string outPath ) {
            outPath = path;
            if( !path.StartsWith("|") ) return false;

            var split = path.Split( "|" );
            if( split.Length != 3 ) return false;

            outPath = split[2];
            return true;
        }

        private static bool GetReplacePath( string gamePath, out string localPath ) {
            localPath = null;
            if( Plugin.AvfxManager?.GetReplacePath( gamePath, out var vfxFile ) == true ) {
                localPath = vfxFile;
                return true;
            }
            else if( Plugin.TextureManager?.GetReplacePath( gamePath, out var texFile ) == true ) {
                localPath = texFile;
                return true;
            }
            else if( Plugin.TmbManager?.GetReplacePath( gamePath, out var tmbFile ) == true ) {
                localPath = tmbFile;
                return true;
            }
            else if( Plugin.PapManager?.GetReplacePath( gamePath, out var papFile ) == true ) {
                localPath = papFile;
                return true;
            }
            else if( Plugin.ScdManager?.GetReplacePath( gamePath, out var scdFile ) == true ) {
                localPath = scdFile;
                return true;
            }
            return false;
        }

        public void ReloadPath( string gamePath, string localPath, List<string> papIds = null ) {
            if( string.IsNullOrEmpty( gamePath ) ) return;

            var gameResource = GetResource( gamePath, true );
            if( Plugin.Configuration?.LogDebug == true && DoDebug( gamePath ) ) PluginLog.Log( "[ReloadPath] {0} {1} -> {1}", gamePath, localPath, gameResource.ToString( "X8" ) );

            if( gameResource != IntPtr.Zero ) {
                InteropUtils.PrepPap( gameResource, papIds );
                RequestFile( GetFileManagerAlt(), gameResource + Constants.GameResourceOffset, gameResource, 1 );
                InteropUtils.WritePapIds( gameResource, papIds );
            }

            if( string.IsNullOrEmpty( localPath  ) ) return;

            var localGameResource = GetResource( gamePath, false ); // get local path resource
            if( Plugin.Configuration?.LogDebug == true && DoDebug( gamePath ) ) PluginLog.Log( "[ReloadPath] {0} {1} -> {1}", gamePath, localPath, localGameResource.ToString( "X8" ) );

            if( localGameResource != IntPtr.Zero ) {
                InteropUtils.PrepPap( localGameResource, papIds );
                RequestFile( GetFileManagerAlt(), localGameResource + Constants.GameResourceOffset, localGameResource, 1 );
                InteropUtils.WritePapIds( localGameResource, papIds );
            }
        }

        private IntPtr GetResource( string path, bool original ) {
            // File type extension
            var ext = path.Split( '.' )[1];
            var charArray = ext.ToCharArray();
            Array.Reverse( charArray );
            var flip = new string( charArray );
            var typeBytes = Encoding.ASCII.GetBytes( flip );
            var bType = stackalloc byte[typeBytes.Length + 1];
            Marshal.Copy( typeBytes, 0, new IntPtr( bType ), typeBytes.Length );
            var pResourceType = ( char* )bType;

            // Category
            var split = path.Split( '/' );
            var categoryString = split[0];
            var categoryBytes = categoryString switch {
                "vfx" => BitConverter.GetBytes( 8u ),
                "chara" => BitConverter.GetBytes( 4u ),
                "bgcommon" => BitConverter.GetBytes( 1u ),
                "sound" => BitConverter.GetBytes( 7u ),
                "bg" => InteropUtils.GetBgCategory( split[1], split[2] ),
                "music" => InteropUtils.GetMusicCategory( split[1] ),
                _ => BitConverter.GetBytes( 0u )
            };
            var bCategory = stackalloc byte[categoryBytes.Length + 1];
            Marshal.Copy( categoryBytes, 0, new IntPtr( bCategory ), categoryBytes.Length );
            var pCategoryId = ( uint* )bCategory;

            ByteString.FromString( path, out var resolvedPath );
            var hash = resolvedPath.GetHashCode();

            var hashBytes = BitConverter.GetBytes( hash );
            var bHash = stackalloc byte[hashBytes.Length + 1];
            Marshal.Copy( hashBytes, 0, new IntPtr( bHash ), hashBytes.Length );
            var pResourceHash = ( int* )bHash;

            var resource = original ? new IntPtr( GetResourceSyncHook.Original( GetFileManager(), pCategoryId, pResourceType, pResourceHash, resolvedPath.Path, null ) ) :
                new IntPtr( GetResourceSyncHandler( GetFileManager(), pCategoryId, pResourceType, pResourceHash, resolvedPath.Path, null ) );
            DecRef( resource );

            return resource;
        }

        private static bool DoDebug( string path ) => path.Contains( ".avfx" ) || path.Contains( ".pap" ) || path.Contains( ".tmb" ) || path.Contains( ".scd" );
    }
}