using Dalamud.Logging;
using Lumina.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select2.Shared {
    public class ParseAvfxFromFile {
        public readonly string OriginalPath;
        public readonly List<string> VfxPaths = new();
        public bool VfxExists => VfxPaths.Count > 0;

        public ParseAvfxFromFile() { }

        public ParseAvfxFromFile( FileResource file ) {
            if( file == null ) return;

            var data = file.Data;
            OriginalPath = file.FilePath.Path;

            var stringData = Encoding.UTF8.GetString( data );
            var matches = SelectUtils.AvfxRegex.Matches( stringData );
            foreach( var m in matches.Cast<Match>() ) {
                VfxPaths.Add( m.Value.Trim( '\u0000' ) );
            }
        }

        public ParseAvfxFromFile( List<FileResource> files ) {
            foreach( var file in files ) {
                var data = file.Data;
                var stringData = Encoding.UTF8.GetString( data );
                var matches = SelectUtils.AvfxRegex.Matches( stringData );
                foreach( var m in matches.Cast<Match>() ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }

        public static void ReadFile( string path, out ParseAvfxFromFile loaded ) {
            loaded = null;
            var result = Plugin.DataManager.FileExists( path );
            if( result ) {
                try {
                    loaded = new ParseAvfxFromFile( Plugin.DataManager.GetFile( path ) );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error reading " + path );
                    return;
                }
            }
            else PluginLog.Error( path + " does not exist" );
        }

        public static void ReadFile( List<string> paths, out ParseAvfxFromFile loaded ) {
            loaded = null;
            var files = new List<FileResource>();
            try {
                foreach( var path in paths ) {
                    if( Plugin.DataManager.FileExists( path ) ) files.Add( Plugin.DataManager.GetFile( path ) );
                }
                loaded = new ParseAvfxFromFile( files );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error reading files" );
                return;
            }
        }
    }
}
