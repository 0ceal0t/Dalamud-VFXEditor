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

        public async void Export(string name, string author, string version, string path, string saveLocation, bool exportAll, bool exportTex )
        {
            try
            {
                List<TTMPL_Simple> simpleParts = new List<TTMPL_Simple>();
                byte[] newData;
                int ModOffset = 0;

                using( MemoryStream ms = new MemoryStream() ) {
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
                                var file = File.Open( tex.localPath, FileMode.Open );
                                long uncompressedLength = file.Length - 80; // .ATEX header is 80 bytes long
                                using( MemoryStream texMs = new MemoryStream()) {
                                    using(BinaryWriter texWriter = new BinaryWriter( texMs ) ) {
                                        using( BinaryReader texReader = new BinaryReader( file ) ) {
                                            var DDSInfo = GetDDSInfo( texReader, tex.Format, tex.Width, tex.Height, tex.MipLevels );
                                            texWriter.Write( MakeType4DatHeader( tex.Format, DDSInfo.mipPartOffsets, DDSInfo.mipPartCounts, ( int )uncompressedLength, tex.MipLevels, tex.Width, tex.Height ) );
                                            texReader.BaseStream.Seek( 0, SeekOrigin.Begin ); // already have the header, no need to do that again
                                            texWriter.Write( texReader.ReadBytes( 80 ) );
                                            texWriter.Write( DDSInfo.compressedDDS.ToArray() );
                                            var modData = texMs.ToArray();
                                            simpleParts.Add( CreateModResource( _path, ModOffset, modData.Length ) );
                                            writer.Write( modData );
                                            ModOffset += modData.Length;
                                        }
                                    }
                                }
                                file.Close();
                            }
                        }

                        if( exportAll ) {
                            foreach(var doc in _plugin.Doc.Docs ) {
                                AddMod( doc.AVFX, doc.Replace.Path );
                            }
                        }
                        else {
                            AddMod( _plugin.AVFX, path);
                        }

                        if( exportTex ) {
                            foreach( KeyValuePair<string, TexReplace> entry in _plugin.Manager.TexManager.GamePathReplace ) {
                                AddTex( entry.Value, entry.Key );
                            }
                        }

                        newData = ms.ToArray();
                    }
                }

                TTMPL mod = new TTMPL();
                mod.TTMPVersion = "1.0s";
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
            simple.Category = "Raw File Copy";
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
            return CreateType2Data( avfx.toAVFX().toBytes() );
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
                using( var cMemoryStream = new MemoryStream() ) {
                    using( var deflateStream = new DeflateStream( cMemoryStream, CompressionMode.Compress ) ) {
                        uMemoryStream.CopyTo( deflateStream );
                        deflateStream.Close();
                        compbytes = cMemoryStream.ToArray();
                    }
                }
                return compbytes;
            }
        }

        public static byte[] MakeType4DatHeader( TextureFormat format, List<short> mipPartOffsets, List<short> mipPartCount, int uncompressedLength, int newMipCount, int newWidth, int newHeight ) {
            var headerData = new List<byte>();
            var headerSize = 24 + ( newMipCount * 20 ) + ( mipPartOffsets.Count * 2 );
            var headerPadding = 128 - ( headerSize % 128 );
            headerData.AddRange( BitConverter.GetBytes( headerSize + headerPadding ) );
            headerData.AddRange( BitConverter.GetBytes( 4 ) );
            headerData.AddRange( BitConverter.GetBytes( uncompressedLength ) );
            headerData.AddRange( BitConverter.GetBytes( 0 ) );
            headerData.AddRange( BitConverter.GetBytes( 0 ) );
            headerData.AddRange( BitConverter.GetBytes( newMipCount ) );
            var partIndex = 0;
            var mipOffsetIndex = 80;
            var uncompMipSize = newHeight * newWidth;
            switch( format ) {
                case TextureFormat.DXT1:
                    uncompMipSize = ( newWidth * newHeight ) / 2;
                    break;
                case TextureFormat.DXT5:
                case TextureFormat.A8:
                    uncompMipSize = newWidth * newHeight;
                    break;
                case TextureFormat.A8R8G8B8:
                default:
                    uncompMipSize = ( newWidth * newHeight ) * 4;
                    break;
            }
            for( var i = 0; i < newMipCount; i++ ) {
                headerData.AddRange( BitConverter.GetBytes( mipOffsetIndex ) );
                var paddedSize = 0;
                for( var j = 0; j < mipPartCount[i]; j++ ) {
                    paddedSize = paddedSize + mipPartOffsets[j + partIndex];
                }
                headerData.AddRange( BitConverter.GetBytes( paddedSize ) );
                headerData.AddRange( uncompMipSize > 16
                    ? BitConverter.GetBytes( uncompMipSize )
                    : BitConverter.GetBytes( 16 ) );
                uncompMipSize = uncompMipSize / 4;
                headerData.AddRange( BitConverter.GetBytes( partIndex ) );
                headerData.AddRange( BitConverter.GetBytes( ( int )mipPartCount[i] ) );
                partIndex = partIndex + mipPartCount[i];
                mipOffsetIndex = mipOffsetIndex + paddedSize;
            }
            foreach( var part in mipPartOffsets ) {
                headerData.AddRange( BitConverter.GetBytes( part ) );
            }
            headerData.AddRange( new byte[headerPadding] );
            return headerData.ToArray();
        }

        public static (List<byte> compressedDDS, List<short> mipPartOffsets, List<short> mipPartCounts) GetDDSInfo( BinaryReader br, TextureFormat format, int newWidth, int newHeight, int newMipCount ) {
            var compressedDDS = new List<byte>();
            var mipPartOffsets = new List<short>();
            var mipPartCount = new List<short>();
            int mipLength;
            switch( format ) {
                case TextureFormat.DXT1:
                    mipLength = ( newWidth * newHeight ) / 2;
                    break;
                case TextureFormat.DXT5:
                case TextureFormat.A8:
                    mipLength = newWidth * newHeight;
                    break;
                case TextureFormat.A8R8G8B8:
                default:
                    mipLength = ( newWidth * newHeight ) * 4;
                    break;
            }

            br.BaseStream.Seek( 80, SeekOrigin.Begin ); // <------------------------

            for( var i = 0; i < newMipCount; i++ ) {
                var mipParts = ( int )Math.Ceiling( mipLength / 16000f );
                mipPartCount.Add( ( short )mipParts );
                if( mipParts > 1 ) {
                    for( var j = 0; j < mipParts; j++ ) {
                        int uncompLength;
                        var comp = true;
                        if( j == mipParts - 1 ) {
                            uncompLength = mipLength % 16000;
                        }
                        else {
                            uncompLength = 16000;
                        }
                        var uncompBytes = br.ReadBytes( uncompLength );
                        var compressed = Compressor( uncompBytes );
                        if( compressed.Length > uncompLength ) {
                            compressed = uncompBytes;
                            comp = false;
                        }
                        compressedDDS.AddRange( BitConverter.GetBytes( 16 ) );
                        compressedDDS.AddRange( BitConverter.GetBytes( 0 ) );
                        compressedDDS.AddRange( !comp
                            ? BitConverter.GetBytes( 32000 )
                            : BitConverter.GetBytes( compressed.Length ) );
                        compressedDDS.AddRange( BitConverter.GetBytes( uncompLength ) );
                        compressedDDS.AddRange( compressed );
                        var padding = 128 - ( compressed.Length % 128 );
                        compressedDDS.AddRange( new byte[padding] );
                        mipPartOffsets.Add( ( short )( compressed.Length + padding + 16 ) );
                    }
                }
                else {
                    int uncompLength;
                    var comp = true;
                    if( mipLength != 16000 ) {
                        uncompLength = mipLength % 16000;
                    }
                    else {
                        uncompLength = 16000;
                    }
                    var uncompBytes = br.ReadBytes( uncompLength );
                    var compressed = Compressor( uncompBytes );
                    if( compressed.Length > uncompLength ) {
                        compressed = uncompBytes;
                        comp = false;
                    }
                    compressedDDS.AddRange( BitConverter.GetBytes( 16 ) );
                    compressedDDS.AddRange( BitConverter.GetBytes( 0 ) );
                    compressedDDS.AddRange( !comp
                        ? BitConverter.GetBytes( 32000 )
                        : BitConverter.GetBytes( compressed.Length ) );
                    compressedDDS.AddRange( BitConverter.GetBytes( uncompLength ) );
                    compressedDDS.AddRange( compressed );
                    var padding = 128 - ( compressed.Length % 128 );
                    compressedDDS.AddRange( new byte[padding] );
                    mipPartOffsets.Add( ( short )( compressed.Length + padding + 16 ) );
                }
                if( mipLength > 32 ) {
                    mipLength = mipLength / 4;
                }
                else {
                    mipLength = 8;
                }
            }
            return (compressedDDS, mipPartOffsets, mipPartCount);
        }
    }
}
