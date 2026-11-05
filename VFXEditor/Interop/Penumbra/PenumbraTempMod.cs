using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Data;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Formats.AtchFormat;
using VfxEditor.Interop.Penumbra.Meta;
using VfxEditor.Ui.Export;

namespace VfxEditor.Interop.Penumbra {
    public class PenumbraTempMod {
        // TODO: ATCH has to be handled differently
        // TODO: warning if multiple documents replace the same thing

        public const string Tag = "VFXEditor";
        public const int Priority = int.MaxValue;
        public static bool Available => Plugin.PenumbraIpc?.PenumbraEnabled == true;
        private static bool Dirty = false;

        private static Dictionary<string, string> ActiveFiles = [];

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
            var atchFiles = new List<(string, AtchFile)>();

            foreach( var (document, gamePath, localPath) in GetActiveDocuments() ) {
                if( gamePath.EndsWith( ".atch", StringComparison.OrdinalIgnoreCase ) ) {
                    if( document is AtchDocument atch && atch.File != null ) atchFiles.Add( (gamePath, atch.File) );
                    continue;
                }

                var key = gamePath.ToLowerInvariant();
                if( resolvedPaths.Contains( key ) ) continue; // first document wins
                if( AddFile( files, key, localPath ) ) resolvedPaths.Add( key );
            }

            meta = PenumbraMetaWriter.ToBase64( atchFiles );
        }

        private static bool Update() {
            if( !Available ) return false;
            // Workspace loads/resets run on a background thread, the mod is updated once they finish
            if( Plugin.State != WorkspaceState.None ) return false;

            GetCurrentFiles( out var files, out var meta );

            if( !Plugin.PenumbraIpc.SetTemporaryMod( Tag, files, meta, Priority ) ) {
                ActiveFiles = [];
                return false;
            }
            ActiveFiles = files;

            if( Plugin.Configuration?.LogDebug == true ) {
                Dalamud.Log( $"[PenumbraTempMod] {files.Count} files, meta: {( string.IsNullOrEmpty( meta ) ? "none" : "atch" )}" );
                foreach( var (game, local) in files ) Dalamud.Log( $"  {game} -> {local}" );
            }
            return true;
        }

        public static void Tick() {
            if( !Dirty ) return;
            if( !Update() ) return;

            Dirty = false;
            var player = Plugin.PlayerObject;
            if( player != null ) Plugin.PenumbraIpc.Redraw( player.ObjectIndex );
        }

        // Push pending changes right away, without redrawing
        public static void Flush() {
            if( !Dirty ) return;
            if( Update() ) Dirty = false;
        }

        // Whether Penumbra is currently redirecting this game path to an existing local file
        public static bool IsRedirected( string gamePath ) {
            if( !Available || Plugin.PenumbraIpc?.ModsEnabled != true || string.IsNullOrEmpty( gamePath ) ) return false;
            Flush();
            if( Dirty ) return false;
            return ActiveFiles.TryGetValue( gamePath.ToLowerInvariant(), out var localPath ) && File.Exists( localPath );
        }

        public static void MarkDirty() {
            Dirty = true;
        }

        public static void Dispose() {
            ActiveFiles = [];
            if( Available ) Plugin.PenumbraIpc.SetTemporaryMod( Tag, [], string.Empty, Priority );
        }
    }
}