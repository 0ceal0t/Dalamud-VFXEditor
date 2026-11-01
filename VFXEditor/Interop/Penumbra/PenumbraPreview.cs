using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Formats.AtchFormat;

namespace VfxEditor.Interop.Penumbra {
    // All in-game previews go through a single Penumbra temporary mod. VFXEditor does not replace files itself.
    //
    // - Every open document maps its game path to the local temp file it was last written to. Each update writes
    //   to a new temp file, so the resolved path (and therefore the resource hash) changes, and the game creates a
    //   fresh resource handle instead of reusing a cached one that other objects may still hold.
    // - Penumbra handles everything VFXEditor used to do by hand: path redirection, custom-path CRC checks for
    //   tex/mdl/scd, inlined .pap loads, the TMB scheduler cache, human.pbd, and redrawing.
    // - .atch files cannot be redirected through Penumbra, so they are sent as ATCH meta manipulations instead.
    //
    // Temporary mods compete with regular mods by priority only (no special precedence). They are added to a
    // collection's cache before regular mods and ties keep the existing entry, so int.MaxValue always wins.
    public static class PenumbraPreview {
        private const string Tag = "VFXEditor";
        public const int Priority = int.MaxValue;
        private static readonly TimeSpan SyncInterval = TimeSpan.FromSeconds( 0.5 );

        private static Dictionary<string, string> RegisteredFiles = null; // null = nothing known to be registered
        private static string RegisteredMeta = null;
        private static DateTime LastSync = DateTime.MinValue;

        public static bool Available => Plugin.PenumbraIpc?.PenumbraEnabled == true;

        // Penumbra resolves UI files only through its Interface collection
        public const string UiCollectionWarning = "Penumbra's Interface collection is set to \"None\", so UI files will not show up in-game. Assign a collection to \"Interface\" in Penumbra's Collections tab (Simple or Individual Assignments)";

        public static bool IsUiPath( string gamePath ) => !string.IsNullOrEmpty( gamePath ) && gamePath.StartsWith( "ui/", StringComparison.OrdinalIgnoreCase );

        // Only warn when Penumbra is loaded; when it is not, PenumbraRequiredWindow already covers it
        public static bool UiPreviewAvailable => !Available || Plugin.PenumbraIpc.InterfaceCollectionAssigned();

        private static void GetCurrent( out Dictionary<string, string> files, out string meta ) {
            files = [];
            meta = string.Empty;
            if( Plugin.State != WorkspaceState.None ) return;

            // Custom (non-vanilla) paths keep resolving to their last local file, even after the document is closed,
            // so files that reference them do not break
            foreach( var (gamePath, localPath) in Plugin.CustomPathBackups ) AddFile( files, gamePath, localPath );

            var atchFiles = new List<(string, AtchFile)>();
            foreach( var group in Plugin.Groups.Where( x => x != null ) ) {
                foreach( var document in group.GetDocuments() ) {
                    if( !document.GetActiveReplacement( out var gamePath, out var localPath ) ) continue;

                    if( gamePath.EndsWith( ".atch", StringComparison.OrdinalIgnoreCase ) ) {
                        if( document is AtchDocument atch && atch.File != null ) atchFiles.Add( (gamePath, atch.File) );
                        continue;
                    }

                    AddFile( files, gamePath, localPath );
                }
            }

            meta = PenumbraMeta.FromAtch( atchFiles );
        }

        private static void AddFile( Dictionary<string, string> files, string gamePath, string localPath ) {
            // Penumbra rejects the whole temporary mod if any path is invalid, so filter here
            if( string.IsNullOrEmpty( gamePath ) || string.IsNullOrEmpty( localPath ) ) return;
            if( gamePath.Length >= 260 || localPath.Length >= 260 ) return;
            if( System.IO.Path.IsPathRooted( gamePath ) || !System.IO.Path.IsPathRooted( localPath ) ) return;
            if( !System.IO.File.Exists( localPath ) ) return;
            files[gamePath.ToLower()] = localPath; // later entries (open documents) win over custom path backups
        }

        private static bool SameFiles( Dictionary<string, string> a, Dictionary<string, string> b ) =>
            a != null && b != null && a.Count == b.Count && a.All( x => b.TryGetValue( x.Key, out var v ) && v == x.Value );

        public static void Invalidate() {
            RegisteredFiles = null;
            RegisteredMeta = null;
        }

        // Returns true if Penumbra now has the current replacements
        public static bool Sync() {
            LastSync = DateTime.Now;
            if( !Available ) {
                Invalidate(); // Penumbra drops temporary mods when it unloads
                return false;
            }

            GetCurrent( out var files, out var meta );
            if( SameFiles( files, RegisteredFiles ) && meta == RegisteredMeta ) return true;
            if( RegisteredFiles == null && files.Count == 0 && string.IsNullOrEmpty( meta ) ) return true;

            if( !Plugin.PenumbraIpc.SetTemporaryMod( Tag, files, meta, Priority ) ) {
                Invalidate();
                return false;
            }

            RegisteredFiles = files;
            RegisteredMeta = meta;
            if( Plugin.Configuration?.LogDebug == true ) {
                Dalamud.Log( $"[PenumbraPreview] {files.Count} files, meta: {( string.IsNullOrEmpty( meta ) ? "none" : "atch" )}" );
                foreach( var (game, local) in files ) Dalamud.Log( $"  {game} -> {local}" );
            }
            return true;
        }

        // Call after a document writes a new file
        public static void UpdateAndRedraw() {
            if( !Sync() ) return;
            var player = Plugin.PlayerObject;
            if( player != null ) Plugin.PenumbraIpc.Redraw( player.ObjectIndex );
        }

        // Penumbra came back (reloaded, or mods re-enabled). Temporary mods do not survive a Penumbra reload, so register again,
        // and redraw so the player picks them up instead of keeping whatever was loaded while Penumbra was unavailable
        public static void Recover() {
            Invalidate();
            if( !Sync() || RegisteredFiles == null ) return; // nothing to apply
            var player = Plugin.PlayerObject;
            if( player != null ) Plugin.PenumbraIpc.Redraw( player.ObjectIndex );
        }

        // Picks up documents being disabled, removed, or workspaces loading
        public static void Tick() {
            if( DateTime.Now - LastSync < SyncInterval ) return;
            Sync();
        }

        public static void Dispose() {
            if( RegisteredFiles != null && Available ) Plugin.PenumbraIpc.SetTemporaryMod( Tag, [], string.Empty, Priority );
            Invalidate();
        }
    }
}
