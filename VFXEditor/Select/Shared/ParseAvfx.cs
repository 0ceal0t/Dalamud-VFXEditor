using Dalamud.Logging;
using Lumina.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VfxEditor.Select.Shared {
    public class ParseAvfx {
        public readonly string OriginalPath;
        public readonly List<string> VfxPaths = new();
        public bool VfxExists => VfxPaths.Count > 0;

        public ParseAvfx() { }

        public ParseAvfx( FileResource file ) {
            if( file == null ) return;

            var data = file.Data;
            OriginalPath = file.FilePath.Path;

            var stringData = Encoding.UTF8.GetString( data );
            var matches = SelectDataUtils.AvfxRegex.Matches( stringData );
            foreach( var m in matches.Cast<Match>() ) {
                VfxPaths.Add( m.Value.Trim( '\u0000' ) );
            }
        }

        public ParseAvfx( List<FileResource> files ) {
            foreach( var file in files ) {
                var data = file.Data;
                var stringData = Encoding.UTF8.GetString( data );
                var matches = SelectDataUtils.AvfxRegex.Matches( stringData );
                foreach( var m in matches.Cast<Match>() ) {
                    VfxPaths.Add( m.Value.Trim( '\u0000' ) );
                }
            }
        }

        public static void ReadFile( string path, out ParseAvfx loaded ) {
            loaded = null;
            if( Plugin.DataManager.FileExists( path ) ) {
                try {
                    loaded = new ParseAvfx( Plugin.DataManager.GetFile( path ) );
                }
                catch( Exception e ) {
                    PluginLog.Error( e, $"Error reading {path}" );
                }
            }
            else PluginLog.Error( $"{path} does not exist" );
        }

        public static void ReadFile( List<string> paths, out ParseAvfx loaded ) {
            loaded = null;
            var files = new List<FileResource>();
            try {
                foreach( var path in paths ) {
                    if( Plugin.DataManager.FileExists( path ) ) files.Add( Plugin.DataManager.GetFile( path ) );
                }
                loaded = new ParseAvfx( files );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error reading files" );
            }
        }

        public static void ReadFile( List<string> paths, out List<ParseAvfx> loaded ) {
            loaded = [];
            try {
                foreach( var path in paths ) {
                    if( Plugin.DataManager.FileExists( path ) ) loaded.Add( new ParseAvfx( Plugin.DataManager.GetFile( path ) ) );
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error reading files" );
            }
        }
    }
}
