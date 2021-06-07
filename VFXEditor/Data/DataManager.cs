using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lumina.Excel.GeneratedSheets;
using System.IO;
using VFXEditor.Data.Texture;
using VFXSelect;

namespace VFXEditor.Data
{
    public class DataManager {
        public Plugin Plugin;
        public TextureManager TexManager;
        public SheetManager Sheets;

        public AVFXNode LastImportNode;

        public DataManager(Plugin plugin ) {
            Plugin = plugin;
            TexManager = new TextureManager( Plugin );
            Sheets = new SheetManager( Plugin.PluginInterface, Path.Combine( Plugin.TemplateLocation, "Files", "npc.csv" ) );
        }

        public bool SaveLocalFile(string path, AVFXBase avfx ) {
            try {
                var node = avfx.ToAVFX();
                var bytes = node.ToBytes();
                File.WriteAllBytes( path, bytes );
            }
            catch(Exception e) {
                PluginLog.LogError(e, "Could not write to file: " + path);
                return false;
            }
            return true;
        }

        public bool GetLocalFile(string path, out AVFXBase avfx) {
            avfx = null;
            if( File.Exists( path ) ) {
                using( BinaryReader br = new BinaryReader( File.Open( path, FileMode.Open ) ) ) {
                    return ReadGameFile( br, out avfx );
                }
            }
            return false;
        }

        public bool GetGameFile(string path, out AVFXBase avfx) {
            avfx = null;
            bool result = Plugin.PluginInterface.Data.FileExists( path );
            if( result )  {
                var file = Plugin.PluginInterface.Data.GetFile( path );
                using(MemoryStream ms = new MemoryStream(file.Data))
                using( BinaryReader br = new BinaryReader( ms ) ) {
                    return ReadGameFile( br, out avfx );
                }
            }
            return false;
        }
        
        public bool ReadGameFile(BinaryReader br, out AVFXBase avfx ) {
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
            catch(Exception e ) {
                PluginLog.LogError( "Error Reading File", e );
                return false;
            }
        }
    }
}
