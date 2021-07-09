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
using System.IO.Compression;
using VFXEditor.Data.Texture;

namespace VFXEditor.External {
    public struct TTMPL {
        public string TTMPVersion;
        public string Name;
        public string Author;
        public string Version;
        public string? Description;
        public string? ModPackPages;
        public TTMPL_Simple[] SimpleModsList;
    }
    public struct TTMPL_Simple {
        public string Name;
        public string Category;
        public string FullPath;
        public bool IsDefault;
        public int ModOffset;
        public int ModSize;
        public string DatFile;
        public string? ModPackEntry;
    }

    public class TexTools {
        public Plugin _plugin;

        public TexTools(Plugin plugin) {
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

        public void Export(string name, string author, string version, string saveLocation, bool exportAll, bool exportTex ) {
            try {
                List<TTMPL_Simple> simpleParts = new List<TTMPL_Simple>();
                byte[] newData;
                int ModOffset = 0;

                using( MemoryStream ms = new MemoryStream() )
                using( BinaryWriter writer = new BinaryWriter( ms ) ) {
                    void AddMod(AVFXBase avfx, string _path) {
                        if( !string.IsNullOrEmpty( _path ) && avfx != null ) {
                            var modData = SquishAVFX( avfx );
                            simpleParts.Add( CreateModResource( _path, ModOffset, modData.Length ) );
                            writer.Write( modData );
                            ModOffset += modData.Length;
                        }
                    }

                    void AddTex(TexReplace tex, string _path ) {
                        if(!string.IsNullOrEmpty(_path) && !string.IsNullOrEmpty( tex.localPath ) ) {
                            using( var file = File.Open( tex.localPath, FileMode.Open ) )
                            using( BinaryReader texReader = new BinaryReader( file ) )
                            using( MemoryStream texMs = new MemoryStream() )
                            using( BinaryWriter texWriter = new BinaryWriter( texMs ) )  {
                                texWriter.Write( CreateType2Data( texReader.ReadBytes( ( int )file.Length ) ) );
                                var modData = texMs.ToArray();
                                simpleParts.Add( CreateModResource( _path, ModOffset, modData.Length ) );
                                writer.Write( modData );
                                ModOffset += modData.Length;
                            }
                        }
                    }

                    if( exportAll ) {
                        foreach(var doc in _plugin.Doc.Docs ) {
                            AddMod( doc.AVFX, doc.Replace.Path );
                        }
                    }
                    else {
                        AddMod( _plugin.AVFX, _plugin.Doc.ActiveDoc.Replace.Path);
                    }

                    if( exportTex ) {
                        foreach( KeyValuePair<string, TexReplace> entry in _plugin.TexManager.GamePathReplace ) {
                            AddTex( entry.Value, entry.Key );
                        }
                    }
                    newData = ms.ToArray();
                }

                TTMPL mod = new TTMPL();
                mod.TTMPVersion = "1.3s";
                mod.Name = name;
                mod.Author = author;
                mod.Version = version;
                mod.Description = null;
                mod.ModPackPages = null;
                mod.SimpleModsList = simpleParts.ToArray();

                string saveDir = Path.GetDirectoryName( saveLocation );
                string tempDir = Path.Combine( saveDir, "VFXEDITOR_TEXTOOLS_TEMP" );
                Directory.CreateDirectory( tempDir );
                string mdpPath = Path.Combine( tempDir, "TTMPD.mpd" );
                string mplPath = Path.Combine(  tempDir, "TTMPL.mpl" );
                string mplString = JsonConvert.SerializeObject( mod );
                File.WriteAllText( mplPath, mplString );
                File.WriteAllBytes( mdpPath, newData );

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

        public TTMPL_Simple CreateModResource( string path, int modOffset, int modSize ) {
            TTMPL_Simple simple = new TTMPL_Simple();
            string[] split = path.Split( '/' );
            simple.Name = split[split.Length - 1];
            simple.Category = "Raw File Import";
            simple.FullPath = path;
            simple.IsDefault = false;
            simple.ModOffset = modOffset;
            simple.ModSize = modSize;

            switch( split[0] ) {
                case "vfx":
                    simple.DatFile = "080000";
                    break;
                case "chara":
                    simple.DatFile = "040000";
                    break;
                case "bgcommon":
                    simple.DatFile = "010000";
                    break;
                case "bg":
                    simple.DatFile = "02";
                    if( split[1] == "ffxiv" ) {
                        simple.DatFile += "0000"; // ok, good to go
                    }
                    else { // like ex1      bg/ex1/03_abr_a2/dun/a2d1/texture/a2d1_b0_silv02_n.tex
                        string exNumber = split[1].Replace( "ex", "" ).PadLeft( 2, '0' );
                        string zoneNumber = split[2].Split( '_' )[0].PadLeft( 2, '0' );
                        simple.DatFile += exNumber;
                        simple.DatFile += zoneNumber;
                    }
                    break;
                default:
                    PluginLog.Log( "Invalid path! Could not find DatFile" );
                    break;
            }
            simple.ModPackEntry = null;
            return simple;
        }

        public byte[] SquishAVFX( AVFXBase avfx ) {
            return CreateType2Data( avfx.ToAVFX().ToBytes() );
        }

        // https://github.com/TexTools/xivModdingFramework/blob/288478772146df085f0d661b09ce89acec6cf72a/xivModdingFramework/SqPack/FileTypes/Dat.cs#L584
        public byte[] CreateType2Data( byte[] dataToCreate ) {
            var newData = new List<byte>();
            var headerData = new List<byte>();
            var dataBlocks = new List<byte>();
            // Header size is defaulted to 128, but may need to change if the data being imported is very large.
            headerData.AddRange( BitConverter.GetBytes( 128 ) );
            headerData.AddRange( BitConverter.GetBytes( 2 ) );
            headerData.AddRange( BitConverter.GetBytes( dataToCreate.Length ) );
            var dataOffset = 0;
            var totalCompSize = 0;
            var uncompressedLength = dataToCreate.Length;
            var partCount = ( int )Math.Ceiling( uncompressedLength / 16000f );
            headerData.AddRange( BitConverter.GetBytes( partCount ) );
            var remainder = uncompressedLength;
            using( var binaryReader = new BinaryReader( new MemoryStream( dataToCreate ) ) ) {
                binaryReader.BaseStream.Seek( 0, SeekOrigin.Begin );
                for( var i = 1; i <= partCount; i++ ) {
                    if( i == partCount ) {
                        var compressedData = Compressor( binaryReader.ReadBytes( remainder ) );
                        var padding = 128 - ( ( compressedData.Length + 16 ) % 128 );
                        dataBlocks.AddRange( BitConverter.GetBytes( 16 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( 0 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( compressedData.Length ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( remainder ) );
                        dataBlocks.AddRange( compressedData );
                        dataBlocks.AddRange( new byte[padding] );
                        headerData.AddRange( BitConverter.GetBytes( dataOffset ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )( ( compressedData.Length + 16 ) + padding ) ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )remainder ) );
                        totalCompSize = dataOffset + ( ( compressedData.Length + 16 ) + padding );
                    }
                    else {
                        var compressedData = Compressor( binaryReader.ReadBytes( 16000 ) );
                        var padding = 128 - ( ( compressedData.Length + 16 ) % 128 );
                        dataBlocks.AddRange( BitConverter.GetBytes( 16 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( 0 ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( compressedData.Length ) );
                        dataBlocks.AddRange( BitConverter.GetBytes( 16000 ) );
                        dataBlocks.AddRange( compressedData );
                        dataBlocks.AddRange( new byte[padding] );
                        headerData.AddRange( BitConverter.GetBytes( dataOffset ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )( ( compressedData.Length + 16 ) + padding ) ) );
                        headerData.AddRange( BitConverter.GetBytes( ( short )16000 ) );
                        dataOffset += ( ( compressedData.Length + 16 ) + padding );
                        remainder -= 16000;
                    }
                }
            }
            headerData.InsertRange( 12, BitConverter.GetBytes( totalCompSize / 128 ) );
            headerData.InsertRange( 16, BitConverter.GetBytes( totalCompSize / 128 ) );
            var headerSize = headerData.Count;
            var rem = headerSize % 128;
            if( rem != 0 ) {
                headerSize += ( 128 - rem );
            }
            headerData.RemoveRange( 0, 4 );
            headerData.InsertRange( 0, BitConverter.GetBytes( headerSize ) );
            var headerPadding = rem == 0 ? 0 : 128 - rem;
            headerData.AddRange( new byte[headerPadding] );
            newData.AddRange( headerData );
            newData.AddRange( dataBlocks );
            return newData.ToArray();
        }

        // https://github.com/TexTools/xivModdingFramework/blob/288478772146df085f0d661b09ce89acec6cf72a/xivModdingFramework/Helpers/IOUtil.cs#L40
        public static byte[] Compressor( byte[] uncompressedBytes ) {
            using( var uMemoryStream = new MemoryStream( uncompressedBytes ) ) {
                byte[] compbytes = null;
                using( var cMemoryStream = new MemoryStream() )
                using( var deflateStream = new DeflateStream( cMemoryStream, CompressionMode.Compress ) ) {
                    uMemoryStream.CopyTo( deflateStream );
                    deflateStream.Close();
                    compbytes = cMemoryStream.ToArray();
                }
                return compbytes;
            }
        }
    }
}
