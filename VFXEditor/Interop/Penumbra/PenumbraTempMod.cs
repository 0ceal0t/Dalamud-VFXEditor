using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VfxEditor.FileManager.Interfaces;

namespace VfxEditor.Interop.Penumbra {
    public class PenumbraTempMod {
        // TODO: UI stuff has to be handled differently
        // TODO: ATCH has to be handled differently
        // TODO: warning if multiple documents replace the same thing

        public const string Tag = "VFXEditor";
        public const int Priority = int.MaxValue;
        public static bool Available => Plugin.PenumbraIpc?.PenumbraEnabled == true;

        private static IEnumerable<(IFileDocument Document, string GamePath, string LocalPath)> GetActiveDocuments() {
            foreach( var group in Plugin.Groups.Where( x => x != null ) ) {
                foreach( var document in group.GetDocuments() ) {
                    if( document.GetActiveReplacement( out var gamePath, out var localPath ) ) yield return (document, gamePath, localPath);
                }
            }
        }

        private static bool AddFile( Dictionary<string, string> files, string gamePath, string localPath ) {
            // Penumbra rejects the whole temporary mod if any path is invalid, so filter here
            if( string.IsNullOrEmpty( gamePath ) || string.IsNullOrEmpty( localPath ) ) return false;
            if( gamePath.Length >= 260 || localPath.Length >= 260 ) return false;
            if( Path.IsPathRooted( gamePath ) || !Path.IsPathRooted( localPath ) ) return false;
            if( !File.Exists( localPath ) ) return false;
            files[gamePath] = localPath;
            return true;
        }

        private static void GetCurrentFiles( out Dictionary<string, string> files, out string meta ) {
            files = [];
            meta = string.Empty;
            if( Plugin.State != WorkspaceState.None ) return;

            var resolvedPaths = new HashSet<string>();
            foreach( var (_, gamePath, localPath) in GetActiveDocuments() ) {
                var key = gamePath.ToLower();
                if( resolvedPaths.Contains( key ) ) continue; // first document wins
                if( AddFile( files, key, localPath ) ) resolvedPaths.Add( key );
            }

            // Custom (non-vanilla) paths keep resolving to their last local file, even after the document is closed,
            // so files that reference them do not break. Open documents take priority
            foreach( var (gamePath, localPath) in Plugin.CustomPathBackups ) {
                if( !resolvedPaths.Contains( gamePath.ToLower() ) ) AddFile( files, gamePath.ToLower(), localPath );
            }
        }

        public static bool Update() {
            if( !Available ) return false;

            GetCurrentFiles( out var files, out var meta );

            if( !Plugin.PenumbraIpc.SetTemporaryMod( Tag, files, meta, Priority ) ) return false;

            if( Plugin.Configuration?.LogDebug == true ) {
                Dalamud.Log( $"[PenumbraTempMod] {files.Count} files, meta: {( string.IsNullOrEmpty( meta ) ? "none" : "atch" )}" );
                foreach( var (game, local) in files ) Dalamud.Log( $"  {game} -> {local}" );
            }
            return true;
        }

        public static void UpdateAndRedraw() {
            if( !Update() ) return;
            var player = Plugin.PlayerObject;
            if( player != null ) Plugin.PenumbraIpc.Redraw( player.ObjectIndex );
        }

        public static void Dispose() {
            if( Available ) Plugin.PenumbraIpc.SetTemporaryMod( Tag, [], string.Empty, Priority );
        }
    }
}