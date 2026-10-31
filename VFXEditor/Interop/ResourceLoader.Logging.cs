using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Resource;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using InteropGenerator.Runtime;
using Penumbra.String;
using Penumbra.String.Classes;
using VfxEditor.Select;

namespace VfxEditor.Interop {
    // File replacement is handled by Penumbra (see PenumbraPreview). These hooks only observe which files the game
    // requests, for "Log all files" and debug output; they never change the request
    public unsafe partial class ResourceLoader {
        public delegate ResourceHandle* GetResourceSyncPrototype( ResourceManager* resourceManager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path,
            void* unknown, void* unkDebugPtr, uint unkDebugInt );

        public delegate ResourceHandle* GetResourceAsyncPrototype( ResourceManager* resourceManager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path,
            void* unknown, bool isUnknown, void* unkDebugPtr, uint unkDebugInt );

        [Signature( Constants.GetResourceSyncSig, DetourName = nameof( GetResourceSyncDetour ) )]
        public readonly Hook<GetResourceSyncPrototype> GetResourceSyncHook = null;

        [Signature( Constants.GetResourceAsyncSig, DetourName = nameof( GetResourceAsyncDetour ) )]
        public readonly Hook<GetResourceAsyncPrototype> GetResourceAsyncHook = null;

        private ResourceHandle* GetResourceSyncDetour(
            ResourceManager* resourceManager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path,
            void* unknown, void* unkDebugPtr, uint unkDebugInt
        ) {
            LogPath( path );
            return GetResourceSyncHook.Original( resourceManager, category, type, hash, path, unknown, unkDebugPtr, unkDebugInt );
        }

        private ResourceHandle* GetResourceAsyncDetour(
            ResourceManager* resourceManager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path,
            void* unknown, bool isUnknown, void* unkDebugPtr, uint unkDebugInt
        ) {
            LogPath( path );
            return GetResourceAsyncHook.Original( resourceManager, category, type, hash, path, unknown, isUnknown, unkDebugPtr, unkDebugInt );
        }

        private static void LogPath( CStringPointer path ) {
            if( Plugin.Configuration?.LogAllFiles != true ) return;
            if( !Utf8GamePath.FromPointer( path, MetaDataComputation.None, out var gamePath ) ) return;

            var gamePathString = gamePath.ToString();
            Dalamud.Log( $"[GetResourceHandler] {gamePathString}" );
            if( SelectDialog.LoggedFiles.Count > 1000 ) SelectDialog.LoggedFiles.Clear();
            SelectDialog.LoggedFiles.Add( gamePathString );
        }
    }
}
