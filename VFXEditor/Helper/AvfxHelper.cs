using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Logging;
using System;
using System.IO;

namespace VFXEditor.Helper {
    public static class AvfxHelper {
        public static AVFXNode LastImportNode { get; private set; } = null;

        public static void Dispose() {
            LastImportNode = null;
        }

        public static bool SaveLocalFile( string path, AVFXBase avfx ) {
            try {
                var node = avfx.ToAVFX();
                var bytes = node.ToBytes();
                File.WriteAllBytes( path, bytes );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Could not write to file: " + path );
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
            var result = Plugin.DataManager.FileExists( path );
            if( result ) {
                var file = Plugin.DataManager.GetFile( path );
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
                    PluginLog.Warning( message );
                }
                if( node == null ) {
                    return false;
                }
                LastImportNode = node;
                avfx = new AVFXBase();
                avfx.Read( node );
                return true;
            }
            catch( Exception e ) {
                PluginLog.Error( "Error Reading File", e );
                return false;
            }
        }
    }
}
