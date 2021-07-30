using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Data {
    public static class DataManager {
        public static AVFXNode LastImportNode => _LastImportNode;

        private static AVFXNode _LastImportNode = null;
        private static DalamudPluginInterface PluginInterface;

        public static void Initialize(Plugin plugin) {
            PluginInterface = plugin.PluginInterface;
        }

        public static void Dispose() {
            PluginInterface = null;
            _LastImportNode = null;
        }

        public static bool SaveLocalFile( string path, AVFXBase avfx ) {
            try {
                var node = avfx.ToAVFX();
                var bytes = node.ToBytes();
                File.WriteAllBytes( path, bytes );
            }
            catch( Exception e ) {
                PluginLog.LogError( e, "Could not write to file: " + path );
                return false;
            }
            return true;
        }

        public static bool GetLocalFile( string path, out AVFXBase avfx ) {
            avfx = null;
            if( File.Exists( path ) ) {
                using var br = new BinaryReader( File.Open( path, FileMode.Open ) );
                return ReadFile( br, out avfx );
            }
            return false;
        }

        public static bool GetGameFile( string path, out AVFXBase avfx ) {
            avfx = null;
            var result = PluginInterface.Data.FileExists( path );
            if( result ) {
                var file = PluginInterface.Data.GetFile( path );
                using var ms = new MemoryStream( file.Data );
                using var br = new BinaryReader( ms );
                return ReadFile( br, out avfx );
            }
            return false;
        }

        private static bool ReadFile( BinaryReader br, out AVFXBase avfx ) {
            avfx = null;
            try {
                var node = AVFXLib.Main.Reader.ReadAVFX( br, out var messages );
                foreach( var message in messages ) {
                    PluginLog.Log( message );
                }
                if( node == null ) {
                    return false;
                }
                _LastImportNode = node;
                avfx = new AVFXBase();
                avfx.Read( node );
                return true;
            }
            catch( Exception e ) {
                PluginLog.LogError( "Error Reading File", e );
                return false;
            }
        }
    }
}
