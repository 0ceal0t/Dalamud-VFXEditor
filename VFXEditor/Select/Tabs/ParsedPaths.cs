using Lumina.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Tabs {
    public class ParsedPaths {
        public readonly string OriginalPath;
        public readonly List<string> Paths = new();

        public ParsedPaths() { }

        public ParsedPaths( FileResource file, Regex regex ) {
            if( file == null ) return;
            OriginalPath = file.FilePath.Path;

            var matches = regex.Matches( Encoding.UTF8.GetString( file.Data ) );
            foreach( var m in matches.Cast<Match>() ) Paths.Add( m.Value.Trim( '\u0000' ) );
        }

        public ParsedPaths( List<FileResource> files, Regex regex ) {
            foreach( var file in files ) {
                var matches = regex.Matches( Encoding.UTF8.GetString( file.Data ) );
                foreach( var m in matches.Cast<Match>() ) Paths.Add( m.Value.Trim( '\u0000' ) );
            }
        }

        public static void ReadFile( string path, Regex regex, out ParsedPaths loaded ) {
            loaded = null;
            if( string.IsNullOrEmpty( path ) ) return;
            if( Dalamud.DataManager.FileExists( path ) ) {
                try {
                    loaded = new ParsedPaths( Dalamud.DataManager.GetFile( path ), regex );
                }
                catch( Exception e ) { Dalamud.Error( e, $"Error reading {path}" ); }
            }
            else Dalamud.Error( $"{path} does not exist" );
        }

        public static void ReadFile( List<string> paths, Regex regex, out ParsedPaths loaded ) {
            loaded = null;
            try {
                var files = new List<FileResource>();
                foreach( var path in paths.Where( x => !string.IsNullOrEmpty( x ) ) ) {
                    if( Dalamud.DataManager.FileExists( path ) ) files.Add( Dalamud.DataManager.GetFile( path ) );
                }
                loaded = new ParsedPaths( files, regex );
            }
            catch( Exception e ) { Dalamud.Error( e, "Error reading files" ); }
        }

        public static void ReadFile( List<string> paths, Regex regex, out List<ParsedPaths> loaded ) {
            loaded = new();
            try {
                foreach( var path in paths.Where( x => !string.IsNullOrEmpty( x ) ) ) {
                    if( Dalamud.DataManager.FileExists( path ) ) loaded.Add( new ParsedPaths( Dalamud.DataManager.GetFile( path ), regex ) );
                }
            }
            catch( Exception e ) { Dalamud.Error( e, "Error reading files" ); }
        }
    }
}
