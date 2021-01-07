using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFXLib.Models;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace VFXEditor
{
    public struct TTMPL
    {
        public string TTMPVersion;
        public string Name;
        public string Author;
        public string Version;
        public string? Description;
        public string? ModPackPages;
        public TTMPL_Simple[] SimpleModsList; 
    }
    public struct TTMPL_Simple
    {
        public string Name;
        public string Category;
        public string FullPath;
        public bool IsDefault;
        public int ModOffset;
        public int ModSize;
        public string DatFile;
        public string? ModPackEntry;
    }

    public class TexTools
    {
        public Plugin _plugin;

        public TexTools(Plugin plugin)
        {
            _plugin = plugin;
            /*
             * TTMPL.mpl ->
             *  {
             *      "TTMPVersion":"1.0s",
             *      "Name":"Ultimate Manatrigger",
             *      "Author":"Gabster",
             *      "Version":"1.0.0",
             *      "Description":null,
             *      "ModPackPages":null,
             *      "SimpleModsList":[
             *          {
             *              "Name":"Ultimate Anarchy", // "Name":"ve0009.avfx","Category":"Raw File Copy"
             *              "Category":"Two Handed",
             *              "FullPath":"chara/weapon/w2501/obj/body/b0027/material/v0001/mt_w2501b0027_a.mtrl",
             *              "IsDefault":false,
             *              "ModOffset":0,
             *              "ModSize":768,
             *              "DatFile":"040000",
             *              "ModPackEntry":null
             *         }
             *     ]
             *  }
             */
        }

        public void Export(string name, string author, string path, string saveLocation, AVFXBase avfx )
        {
            try
            {
                var data = avfx.toAVFX().toBytes();

                TTMPL_Simple simple = new TTMPL_Simple();
                string[] split = path.Split( '/' );
                simple.Name = split[split.Length - 1];
                simple.Category = "Raw File Copy";
                simple.FullPath = path;
                simple.IsDefault = false;
                simple.ModOffset = 0;
                simple.ModSize = data.Length;
                switch( split[0] )
                {
                    case "vfx":
                        simple.DatFile = "080000";
                        break;
                    case "chara":
                        simple.DatFile = "040000";
                        break;
                    default:
                        PluginLog.Log( "Invalid VFX path! Could not find DatFile" );
                        return;
                }
                simple.ModPackEntry = null;

                TTMPL mod = new TTMPL();
                mod.TTMPVersion = "1.0s";
                mod.Name = name;
                mod.Author = author;
                mod.Version = "1.0.0";
                mod.Description = null;
                mod.ModPackPages = null;
                TTMPL_Simple[] simples = { simple };
                mod.SimpleModsList = simples;

                string saveDir = Path.GetDirectoryName( saveLocation );
                string tempDir = Path.Combine( new string[] { saveDir, "VFXEDITOR_TEXTOOLS_TEMP" } );
                Directory.CreateDirectory( tempDir );
                string mdpPath = Path.Combine( new string[] { tempDir, "TTMPD.mpd" } );
                string mplPath = Path.Combine( new string[] { tempDir, "TTMPL.mpl" } );
                string mplString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( mplPath, mplString );
                File.WriteAllBytes( mdpPath, data );

                FastZip zip = new FastZip();
                zip.CreateEmptyDirectories = true;
                zip.CreateZip( saveLocation, tempDir, false, "" );

                Directory.Delete( tempDir, true);

                PluginLog.Log( "Exported To: " + saveLocation );
            }
            catch(Exception e )
            {
                PluginLog.LogError( e, "Could not export to TexTools" );
            }
        }
    }
}
