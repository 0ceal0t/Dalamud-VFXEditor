using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Select;

namespace VfxEditor.Utils {
    public enum GamePathStatus {
        Valid,   // exists in the game files
        Custom,  // custom path, but not invalid
        Invalid  // malformed or the wrong file type
    }

    public static class GamePathUtils {
        private static readonly Dictionary<string, bool> GameFileCache = new( StringComparer.OrdinalIgnoreCase );

        public static bool GameFileExists( string path ) {
            if( string.IsNullOrEmpty( path ) ) return false;
            if( GameFileCache.TryGetValue( path, out var exists ) ) return exists;
            if( GameFileCache.Count > 1000 ) GameFileCache.Clear();
            exists = Dalamud.GameFileExists( path );
            GameFileCache[path] = exists;
            return exists;
        }

        public static bool IsWellFormed( string path, string extension, out string error ) {
            error = null;
            if( string.IsNullOrWhiteSpace( path ) ) error = "Path is empty";
            else if( path.Length >= 260 ) error = "Path is too long";
            else if( Path.IsPathRooted( path ) || path.Contains( '\\' ) || path.Contains( ':' ) ) error = "Not a game path";
            else if( !string.IsNullOrEmpty( extension ) && !path.EndsWith( $".{extension}", StringComparison.OrdinalIgnoreCase ) ) error = $"Wrong file type, expected .{extension}";
            return error == null;
        }

        public static GamePathStatus GetStatus( string path, string extension, out string message ) {
            if( !IsWellFormed( path, extension, out message ) ) return GamePathStatus.Invalid;
            if( GameFileExists( path ) ) {
                message = "";
                return GamePathStatus.Valid;
            }
            message = "Path does not exist in the game files";
            return GamePathStatus.Custom;
        }

        // Whether the game will actually find something when it tries to load this path right now
        public static bool CanLoad( string path ) {
            if( GameFileExists( path ) ) return true;
            if( PenumbraTempMod.IsRedirected( path ) ) return true;

            // Another Penumbra mod could also be providing it
            var resolved = Plugin.PenumbraIpc?.ResolveDefaultPath( path );
            return !string.IsNullOrEmpty( resolved ) && Path.IsPathRooted( resolved ) && File.Exists( resolved );
        }

        public static GamePathStatus GetReplaceStatus( SelectResult replace, string extension, out string message ) {
            message = null;
            if( replace == null ) return GamePathStatus.Valid;
            return GetStatus( replace.Path, extension, out message );
        }

        public static Vector4 GetStatusColor( GamePathStatus status ) => status switch {
            GamePathStatus.Valid => UiUtils.GREEN_COLOR,
            GamePathStatus.Custom => UiUtils.DALAMUD_ORANGE,
            _ => UiUtils.DALAMUD_RED
        };
    }
}