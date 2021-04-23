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
using VFXEditor.Data.Sheets;

namespace VFXEditor.Data
{
    public class DataManager
    {
        public Plugin _plugin;
        public TextureManager TexManager;
        public string TempPath;
        public string NpcCsv;

        public ItemSheetLoader _Items;
        public ActionSheetLoader _Actions;
        public CutsceneSheetLoader _Cutscenes;
        public EmoteSheetLoader _Emotes;
        public GimmickSheetLoader _Gimmicks;
        public NonPlayerActionSheetLoader _NonPlayerActions;
        public NpcSheetLoader _Npcs;
        public StatusSheetLoader _Statuses;
        public ZoneSheetLoader _Zones;
        public MountSheeetLoader _Mounts;

        public DataManager(Plugin plugin )
        {
            _plugin = plugin;
            TexManager = new TextureManager( _plugin );
            NpcCsv = Path.Combine( Plugin.TemplateLocation, "Files", "npc.csv" );

            _Items = new ItemSheetLoader( this, plugin );
            _Actions = new ActionSheetLoader( this, plugin );
            _Cutscenes = new CutsceneSheetLoader( this, plugin );
            _Emotes = new EmoteSheetLoader( this, plugin );
            _Gimmicks = new GimmickSheetLoader( this, plugin );
            _NonPlayerActions = new NonPlayerActionSheetLoader( this, plugin );
            _Npcs = new NpcSheetLoader( this, plugin );
            _Statuses = new StatusSheetLoader( this, plugin );
            _Zones = new ZoneSheetLoader( this, plugin );
            _Mounts = new MountSheeetLoader( this, plugin );
        }


        // ======  EXPORT AVFX  ======
        public bool SaveLocalFile(string path, AVFXBase avfx ) {
            try {
                var node = avfx.toAVFX();
                var bytes = node.toBytes();
                File.WriteAllBytes( path, bytes );
            }
            catch(Exception ex ) {
                PluginLog.LogError( "Could not write to file: " + path );
                PluginLog.LogError( ex.ToString() );
                return false;
            }
            return true;
        }

        // ====== LOCAL AVFX =====
        public AVFXNode LastImportNode = null;
        public bool GetLocalFile(string path, out AVFXBase avfx) {
            avfx = null;
            if( File.Exists( path ) ) {
                using( BinaryReader br = new BinaryReader( File.Open( path, FileMode.Open ) ) ) {
                    return ReadGameFile( br, out avfx );
                }
            }
            return false;
        }
        // ===== GAME AVFX ======
        public bool GetGameFile(string path, out AVFXBase avfx) {
            avfx = null;
            bool result = _plugin.PluginInterface.Data.FileExists( path );
            if( result )  {
                var file = _plugin.PluginInterface.Data.GetFile( path );
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
                AVFXNode node = AVFXLib.Main.Reader.readAVFX( br, out List<string> messages );
                foreach( string message in messages ) {
                    PluginLog.Log( message );
                }
                if( node == null ) {
                    return false;
                }
                LastImportNode = node;
                avfx = new AVFXBase();
                avfx.read( node );
                return true;
            }
            catch(Exception e ) {
                PluginLog.LogError( "Error Reading File", e );
                return false;
            }
        }
    }
}
