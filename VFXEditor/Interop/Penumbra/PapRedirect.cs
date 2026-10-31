using System;
using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.Interop.Penumbra {
    // Reloading .pap files in place is fragile: the same resource handle can be held by several objects,
    // and most .pap loads (motion packs) inline GetResourceAsync, so they never go through our GetResource hooks
    // and keep resolving to the cached handle for the game path.
    //
    // Penumbra already rewrites those inlined loads to resolve the path before the CRC is computed. By pointing a
    // temporary mod at the newest temp file, each update resolves to a different path -> different hash -> fresh
    // resource handle, so a redraw picks up the new animations no matter who still holds the old handle.
    public static class PapRedirect {
        private const string Tag = "VFXEditor_Pap";
        private const int Priority = 99;
        private static readonly TimeSpan SyncInterval = TimeSpan.FromSeconds( 0.5 );

        private static Dictionary<string, string> Registered = null; // null = nothing known to be registered
        private static DateTime LastSync = DateTime.MinValue;

        public static bool Available => Plugin.PenumbraIpc?.PenumbraEnabled == true && Plugin.Configuration?.UsePenumbraPapRedirect != false;

        private static Dictionary<string, string> GetRedirects() {
            var ret = new Dictionary<string, string>();
            if( Plugin.State != WorkspaceState.None || Plugin.PapManager == null ) return ret;

            foreach( var document in Plugin.PapManager.Documents ) {
                if( document.File == null ) continue;
                var gamePath = document.ReplacePath; // empty when disabled
                if( string.IsNullOrEmpty( gamePath ) || string.IsNullOrEmpty( document.WriteLocation ) ) continue;
                ret[gamePath.ToLower()] = document.WriteLocation; // later documents win, same as GetReplacePath ordering is not guaranteed anyway
            }
            return ret;
        }

        private static bool Same( Dictionary<string, string> a, Dictionary<string, string> b ) =>
            a != null && b != null && a.Count == b.Count && a.All( x => b.TryGetValue( x.Key, out var v ) && v == x.Value );

        // Returns true if Penumbra now has the current redirects
        public static bool Sync() {
            LastSync = DateTime.Now;
            if( !Available ) {
                Registered = null; // Penumbra drops temporary mods when it unloads, so re-register when it comes back
                return false;
            }

            var redirects = GetRedirects();
            if( Same( redirects, Registered ) ) return true;
            if( Registered == null && redirects.Count == 0 ) return true;

            if( !Plugin.PenumbraIpc.SetTemporaryMod( Tag, redirects, Priority ) ) {
                Registered = null;
                return false;
            }

            Registered = redirects;
            if( Plugin.Configuration?.LogDebug == true ) Dalamud.Log( $"[PapRedirect] {string.Join( ", ", redirects.Select( x => $"{x.Key} -> {x.Value}" ) )}" );
            return true;
        }

        // Called after a .pap document writes a new temp file
        public static bool UpdateAndRedraw() {
            if( !Sync() ) return false;

            var player = Plugin.PlayerObject;
            if( player == null ) return true;
            return Plugin.PenumbraIpc.Redraw( player.ObjectIndex );
        }

        // Picks up documents being disabled, removed, or workspaces loading
        public static void Tick() {
            if( DateTime.Now - LastSync < SyncInterval ) return;
            Sync();
        }

        public static void Dispose() {
            if( Registered != null && Plugin.PenumbraIpc?.PenumbraEnabled == true ) {
                Plugin.PenumbraIpc.SetTemporaryMod( Tag, new Dictionary<string, string>(), Priority );
            }
            Registered = null;
        }
    }
}
