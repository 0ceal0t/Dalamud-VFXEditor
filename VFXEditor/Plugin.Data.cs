using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor {
    public partial class Plugin {
        public AVFXNode LastImportNode;

        public bool SaveLocalFile( string path, AVFXBase avfx ) {
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

        public bool GetLocalFile( string path, out AVFXBase avfx ) {
            avfx = null;
            if( File.Exists( path ) ) {
                using( BinaryReader br = new BinaryReader( File.Open( path, FileMode.Open ) ) ) {
                    return ReadGameFile( br, out avfx );
                }
            }
            return false;
        }

        public bool GetGameFile( string path, out AVFXBase avfx ) {
            avfx = null;
            bool result = PluginInterface.Data.FileExists( path );
            if( result ) {
                var file = PluginInterface.Data.GetFile( path );
                using( MemoryStream ms = new MemoryStream( file.Data ) )
                using( BinaryReader br = new BinaryReader( ms ) ) {
                    return ReadGameFile( br, out avfx );
                }
            }
            return false;
        }

        public bool ReadGameFile( BinaryReader br, out AVFXBase avfx ) {
            avfx = null;
            try {
                AVFXNode node = AVFXLib.Main.Reader.ReadAVFX( br, out List<string> messages );
                foreach( string message in messages ) {
                    PluginLog.Log( message );
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
                PluginLog.LogError( "Error Reading File", e );
                return false;
            }
        }
    }
}
