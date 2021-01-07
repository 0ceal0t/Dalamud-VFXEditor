using AVFXLib.Models;
using Dalamud.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor
{
    public struct PenumbraMod
    {
        public string Name;
        public string Author;
        public string Description;
        public string? Version;
        public string? Website;
        public Dictionary<string, string> FileSwaps;
    }

    public class Penumbra
    {
        public Plugin _plugin;

        public Penumbra(Plugin plugin )
        {
            _plugin = plugin;
            /*
             * {
             *  "Name":"Ultimate Manatrigger",
             *  "Author":"Gabster",
             *  "Description":"Mod imported from TexTools mod pack",
             *  "Version":null,
             *  "Website":null,
             *  "FileSwaps":{}
             *  }
             */
        }

        public void Export( string name, string author, string path, string saveLocation, AVFXBase avfx )
        {
            try
            {
                var data = avfx.toAVFX().toBytes();

                PenumbraMod mod = new PenumbraMod();
                mod.Name = name;
                mod.Author = author;
                mod.Description = "Exported from VFXEditor";
                mod.Version = null;
                mod.Website = null;
                mod.FileSwaps = new Dictionary<string, string>();

                string modFolder = Path.Combine( saveLocation, name );
                string modConfig = Path.Combine( modFolder, "meta.json" );
                string modFile = Path.Combine( modFolder, path );
                string modFileFolder = Path.GetDirectoryName( modFile );

                Directory.CreateDirectory( modFolder );
                string configString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( modConfig, configString );

                Directory.CreateDirectory( modFileFolder );
                File.WriteAllBytes( modFile, data );

                PluginLog.Log( "Exported To: " + saveLocation );
            }
            catch( Exception e )
            {
                PluginLog.LogError( e, "Could not export to Penumbra" );
            }
        }
    }
}
