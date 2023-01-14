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
        private bool IsEnabled;

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
        public delegate IntPtr StaticVfxCreateDelegate2( char* path, char* pool );
        public Hook<StaticVfxCreateDelegate2> StaticVfxCreateHook { get; private set; }

        public delegate IntPtr StaticVfxRemoveDelegate2( IntPtr vfx );
        public Hook<StaticVfxRemoveDelegate2> StaticVfxRemoveHook { get; private set; }

        // ======== ACTOR =============
        public delegate IntPtr ActorVfxCreateDelegate( string a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );
        public ActorVfxCreateDelegate ActorVfxCreate;

        public delegate IntPtr ActorVfxRemoveDelegate( IntPtr vfx, char a2 );
        public ActorVfxRemoveDelegate ActorVfxRemove;

        // ======== ACTOR HOOKS =============
        public delegate IntPtr ActorVfxCreateDelegate2( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 );
        public Hook<ActorVfxCreateDelegate2> ActorVfxCreateHook { get; private set; }

        public delegate IntPtr ActorVfxRemoveDelegate2( IntPtr vfx, char a2 );
        public Hook<ActorVfxRemoveDelegate2> ActorVfxRemoveHook { get; private set; }

        // ========= MISC ==============
        public delegate IntPtr GetMatrixSingletonDelegate();
        public GetMatrixSingletonDelegate GetMatrixSingleton { get; private set; }

        public delegate IntPtr GetFileManagerDelegate();
        private GetFileManagerDelegate GetFileManager;
        private GetFileManagerDelegate GetFileManager2;

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
            var actorVfxRemoveAddress_1 = scanner.ScanText( Constants.ActorVfxRemoveSig ) + 7;
            var actorVfxRemoveAddress = Marshal.ReadIntPtr( actorVfxRemoveAddress_1 + Marshal.ReadInt32( actorVfxRemoveAddress_1 ) + 4 );

            ActorVfxCreate = Marshal.GetDelegateForFunctionPointer<ActorVfxCreateDelegate>( actorVfxCreateAddress );
            ActorVfxRemove = Marshal.GetDelegateForFunctionPointer<ActorVfxRemoveDelegate>( actorVfxRemoveAddress );
            StaticVfxRemove = Marshal.GetDelegateForFunctionPointer<StaticVfxRemoveDelegate>( staticVfxRemoveAddress );
            StaticVfxRun = Marshal.GetDelegateForFunctionPointer<StaticVfxRunDelegate>( scanner.ScanText( Constants.StaticVfxRunSig ) );
            StaticVfxCreate = Marshal.GetDelegateForFunctionPointer<StaticVfxCreateDelegate>( staticVfxCreateAddress );

            StaticVfxCreateHook = Hook<StaticVfxCreateDelegate2>.FromAddress( staticVfxCreateAddress, StaticVfxNewHandler );
            StaticVfxRemoveHook = Hook<StaticVfxRemoveDelegate2>.FromAddress( staticVfxRemoveAddress, StaticVfxRemoveHandler );
            ActorVfxCreateHook = Hook<ActorVfxCreateDelegate2>.FromAddress( actorVfxCreateAddress, ActorVfxNewHandler );
            ActorVfxRemoveHook = Hook<ActorVfxRemoveDelegate2>.FromAddress( actorVfxRemoveAddress, ActorVfxRemoveHandler );

            GetMatrixSingleton = Marshal.GetDelegateForFunctionPointer<GetMatrixSingletonDelegate>( scanner.ScanText( Constants.GetMatrixSig ) );
            GetFileManager = Marshal.GetDelegateForFunctionPointer<GetFileManagerDelegate>( scanner.ScanText( Constants.GetFileManagerSig ) );
            GetFileManager2 = Marshal.GetDelegateForFunctionPointer<GetFileManagerDelegate>( scanner.ScanText( Constants.GetFileManager2Sig ) );
            DecRef = Marshal.GetDelegateForFunctionPointer<DecRefDelegate>( scanner.ScanText( Constants.DecRefSig ) );
            RequestFile = Marshal.GetDelegateForFunctionPointer<RequestFileDelegate>( scanner.ScanText( Constants.RequestFileSig ) );
        }

        private IntPtr StaticVfxNewHandler( char* path, char* pool ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( path ), Encoding.ASCII, 256 );
            var vfx = StaticVfxCreateHook.Original( path, pool );
            Plugin.VfxTracker?.AddStatic( ( VfxStruct* )vfx, vfxPath );

            if (Plugin.Configuration?.LogVfxDebug == true) PluginLog.Log( $"{vfxPath} {vfx:X8}" );

            return vfx;
        }

        private IntPtr StaticVfxRemoveHandler( IntPtr vfx ) {
            if( Plugin.Spawn != null && vfx == ( IntPtr )Plugin.Spawn.Vfx ) {
                Plugin.ClearSpawn();
            }
            Plugin.VfxTracker?.RemoveStatic( ( VfxStruct* )vfx );
            return StaticVfxRemoveHook.Original( vfx );
        }

        private IntPtr ActorVfxNewHandler( char* a1, IntPtr a2, IntPtr a3, float a4, char a5, ushort a6, char a7 ) {
            var vfxPath = Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( a1 ), Encoding.ASCII, 256 );
            var vfx = ActorVfxCreateHook.Original( a1, a2, a3, a4, a5, a6, a7 );
            Plugin.VfxTracker?.AddActor( ( VfxStruct* )vfx, vfxPath );

            if( Plugin.Configuration?.LogVfxDebug == true ) PluginLog.Log( $"{vfxPath} {vfx:X8}" );

            return vfx;
        }

        private IntPtr ActorVfxRemoveHandler( IntPtr vfx, char a2 ) {
            if( Plugin.Spawn != null && vfx == ( IntPtr )Plugin.Spawn.Vfx ) {
                Plugin.ClearSpawn();
            }
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
            var player = Plugin.ClientState.LocalPlayer;
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
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            int* pResourceHash,
            byte* pPath,
            GetResourceParameters* pUnknown
        ) => GetResourceHandler( true, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, false );

        private void* GetResourceAsyncHandler(
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            int* pResourceHash,
            byte* pPath,
            GetResourceParameters* pUnknown,
            bool isUnknown
        ) => GetResourceHandler( false, pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );

        private void* CallOriginalHandler(
            bool isSync,
            IntPtr pFileManager,
            uint* pCategoryId,
            char* pResourceType,
            int* pResourceHash,
            byte* pPath,
            GetResourceParameters* pUnknown,
            bool isUnknown
        ) => isSync
            ? GetResourceSyncHook.Original( pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown )
            : GetResourceAsyncHook.Original( pFileManager, pCategoryId, pResourceType, pResourceHash, pPath, pUnknown, isUnknown );

        private void* GetResourceHandler(
            bool isSync, IntPtr fileManager,
            uint* categoryId,
            char* resourceType,
            int* resourceHash,
            byte* path,
            GetResourceParameters* resParams,
            bool isUnknown
        ) {
            var gameFsPath = Marshal.PtrToStringAnsi( new IntPtr( path ) );

            if( Plugin.Configuration?.LogAllFiles == true ) PluginLog.Log( "[GetResourceHandler] {0}", gameFsPath );

            var fsPath = GetReplacePath( gameFsPath, out var localPath ) ? localPath : null;

            if( fsPath == null || fsPath.Length >= 260 ) {
                var value = CallOriginalHandler( isSync, fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );
                if( Plugin.Configuration?.LogDebug == true && DoDebug( gameFsPath ) ) PluginLog.Log( "[GetResourceHandler] {0} -> {1} -> {2}", gameFsPath, fsPath, new IntPtr( value ).ToString( "X8" ) );
                return value;
            }

            var resolvedPath = new FullPath( fsPath );

            *resourceHash = ComputeHash( resolvedPath.InternalName, resParams );
            path = resolvedPath.InternalName.Path;

            var value2 = CallOriginalHandler( isSync, fileManager, categoryId, resourceType, resourceHash, path, resParams, isUnknown );
            if( Plugin.Configuration?.LogDebug == true && DoDebug( gameFsPath ) ) PluginLog.Log( "[GetResourceHandler] Replace {0} -> {1} -> {2} / {3}", gameFsPath, fsPath, new IntPtr( value2 ).ToString( "X8" ), Marshal.PtrToStringAnsi( new IntPtr( path ) ) );
            return value2;
        }

        private static int ComputeHash( ByteString path, GetResourceParameters* resParams ) {
            if( resParams == null || !resParams->IsPartialRead ) return path.Crc32;

            return ByteString.Join(
                ( byte )'.',
                path,
                ByteString.FromStringUnsafe( resParams->SegmentOffset.ToString( "x" ), true ),
                ByteString.FromStringUnsafe( resParams->SegmentLength.ToString( "x" ), true )
            ).Crc32;
        }

        private byte ReadSqpackHandler( IntPtr pFileHandler, SeFileDescriptor* pFileDesc, int priority, bool isSync ) {
            var gameFsPath = GetString( pFileDesc->ResourceHandle->File );

            var isRooted = Path.IsPathRooted( gameFsPath );

            // looking for refreshed paths
            if( gameFsPath != null && !isRooted ) {
                var replacementPath = GetReplacePath( gameFsPath, out var localPath ) ? localPath : null;
                if( replacementPath != null && Path.IsPathRooted( replacementPath ) && replacementPath.Length < 260 ) {
                    gameFsPath = replacementPath;
                    isRooted = true;
                }
            }

            if( gameFsPath == null || gameFsPath.Length >= 260 || !isRooted ) {
                return ReadSqpackHook.Original( pFileHandler, pFileDesc, priority, isSync );
            }

            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "[ReadSqpackHandler] Replaced with {0}", gameFsPath );

            pFileDesc->FileMode = FileMode.LoadUnpackedResource;

            // note: must be utf16
            var utfPath = Encoding.Unicode.GetBytes( gameFsPath );
            Marshal.Copy( utfPath, 0, new IntPtr( &pFileDesc->UtfFileName ), utfPath.Length );
            var fd = stackalloc byte[0x20 + utfPath.Length + 0x16];
            Marshal.Copy( utfPath, 0, new IntPtr( fd + 0x21 ), utfPath.Length );
            pFileDesc->FileDescriptor = fd;

            return ReadFile( pFileHandler, pFileDesc, priority, isSync );
        }

        private static string GetString( StdString str ) {
            var len = ( int )str.Size;
            if( len > 15 ) {
                return Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( str.BufferPtr ), Encoding.ASCII, len );
            }
            return Dalamud.Memory.MemoryHelper.ReadString( new IntPtr( &str.BufferPtr ), Encoding.ASCII, len );
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
            else if( Plugin.TmbManager.GetReplacePath( gamePath, out var tmbFile ) == true ) {
                localPath = tmbFile;
                return true;
            }
            else if( Plugin.PapManager.GetReplacePath( gamePath, out var papFile ) == true ) {
                localPath = papFile;
                return true;
            }
            else if( Plugin.ScdManager.GetReplacePath( gamePath, out var scdFile ) == true ) {
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
                PrepPap( gameResource, papIds );
                RequestFile( GetFileManager2(), gameResource + Constants.GameResourceOffset, gameResource, 1 );
                WritePapIds( gameResource, papIds );
            }

            if( !string.IsNullOrEmpty( localPath ) ) {
                var gameResource2 = GetResource( gamePath, false ); // get local path resource
                if( Plugin.Configuration?.LogDebug == true && DoDebug( gamePath ) ) PluginLog.Log( "[ReloadPath] {0} {1} -> {1}", gamePath, localPath, gameResource2.ToString( "X8" ) );

                if( gameResource2 != IntPtr.Zero ) {
                    PrepPap( gameResource2, papIds );
                    RequestFile( GetFileManager2(), gameResource2 + Constants.GameResourceOffset, gameResource2, 1 );
                    WritePapIds( gameResource2, papIds );
                }
            }
        }

        private static void PrepPap( IntPtr resource, List<string> papIds ) {
            if( papIds == null ) return;
            Marshal.WriteByte( resource + 105, 0xec );
        }

        private static void WritePapIds( IntPtr resource, List<string> papIds ) {
            if( papIds == null ) return;
            var data = Marshal.ReadIntPtr( resource + Constants.PapIdsOffset );
            for( var i = 0; i < papIds.Count; i++ ) {
                SafeMemory.WriteString( data + ( i * 40 ), papIds[i], Encoding.ASCII );
                Marshal.WriteByte( data + ( i * 40 ) + 34, (byte)i );
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
                "bg" => GetBgCategory( split[1], split[2] ),
                "music" => GetMusicCategory( split[1] ),
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

        private static byte[] GetBgCategory( string expansion, string zone ) {
            var ret = BitConverter.GetBytes( 2u );
            if( expansion == "ffxiv" ) return ret;
            // ex1/03_abr_a2/fld/a2f1/level/a2f1 -> [02 00 03 01]
            // expansion = ex1
            // zone = 03_abr_a2
            var expansionTrimmed = expansion.Replace( "ex", "" );
            var zoneTrimmed = zone.Split( '_' )[0];
            ret[2] = byte.Parse( zoneTrimmed );
            ret[3] = byte.Parse( expansionTrimmed );
            return ret;
        }

        private static byte[] GetMusicCategory( string expansion ) {
            var ret = BitConverter.GetBytes( 12u );
            if( expansion == "ffxiv" ) return ret;
            // music/ex4/BGM_EX4_Field_Ult_Day03.scd
            // 04 00 00 0C
            var expansionTrimmed = expansion.Replace( "ex", "" );
            ret[3] = byte.Parse( expansionTrimmed );
            return ret;
        }

        private static bool DoDebug( string path ) => path.Contains( ".avfx" ) || path.Contains( ".pap" ) || path.Contains( ".tmb" ) || path.Contains( ".scd" );
    }
}