using AVFXLib.AVFX;
using AVFXLib.Models;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumina.Excel.GeneratedSheets;
using System.IO;
using System.Collections.Concurrent;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VFXEditor.Data.Texture;

namespace VFXEditor
{
    public struct TexData { // used for the texture previews
        public byte[] Data;
        public ushort Height;
        public ushort Width;
        public ushort MipLevels;
        public ushort Depth;
        public bool IsReplaced;
        public TextureFormat Format;
        public ImGuiScene.TextureWrap Wrap;
    }

    public struct TexReplace {
        public string localPath;
        public int Height;
        public int Width;
        public int Depth;
        public int MipLevels;
        public TextureFormat Format;
    }

    public class TextureManager
    {
        public Plugin _plugin;
        public string WriteLocation;
        public int TEX_ID = 0;

        public ConcurrentDictionary<string, TexData> PathToTex = new ConcurrentDictionary<string, TexData>(); // for previewing textures
        public ConcurrentDictionary<string, TexReplace> GamePathReplace = new ConcurrentDictionary<string, TexReplace>();

        public TextureManager(Plugin plugin ) {
            _plugin = plugin;
            WriteLocation = Path.Combine( plugin.WriteLocation, "documents/textures" );
            Directory.CreateDirectory( WriteLocation );
        }

        public void Reset() {
            foreach(KeyValuePair<string, TexData> entry in PathToTex ) {
                if(PathToTex.TryRemove( entry.Key, out var data ) ) {
                    data.Wrap?.Dispose();
                }
            }
        }

        public bool GetLocalPath(string gamePath, out FileInfo file ) {
            file = null;
            if( !GamePathReplace.ContainsKey( gamePath ) ) {
                return false;
            }
            file = new FileInfo( GamePathReplace[gamePath].localPath );
            return true;
        }

        /*
         * UI
         * reset button next to preview
         * textools export
         * penumbra export
         */

        // https://github.com/TexTools/xivModdingFramework/blob/872329d84c7b920fe2ac5e0b824d6ec5b68f4f57/xivModdingFramework/Textures/FileTypes/Tex.cs
        public bool ImportTexture(string fileLocation, string replacePath ) {
            if( !_plugin.PluginInterface.Data.FileExists( replacePath ) ) {
                PluginLog.Log( $"{replacePath} does not exist" );
                return false;
            }

            try {
                string path = Path.Combine( WriteLocation, "TexTemp" + ( TEX_ID++ ) + ".atex" );
                TexReplace replaceData;

                bool isDDS = Path.GetExtension( fileLocation ).ToLower() == ".dds";
                if( isDDS ) {
                    // get format from file
                    var ddsFile = DDSFile.Read( fileLocation );
                    var format = VFXTexture.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null )
                        return false;
                    using( BinaryWriter writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateAtex( format, ddsFile, writer );
                    }
                    ddsFile.Dispose();
                }
                else {
                    // get format from internal file
                    var texFile = _plugin.PluginInterface.Data.GetFile<VFXTexture>( replacePath );

                    using( var surface = Surface.LoadFromFile( fileLocation ) ) {
                        surface.FlipVertically();
                        var maxMipCount = -1;

                        using( var compressor = new Compressor() ) {
                            CompressionFormat compFormat = VFXTexture.TextureToCompressionFormat( texFile.Header.Format );
                            if( compFormat == CompressionFormat.ETC1 ) {
                                return false;
                            }

                            compressor.Input.SetMipmapGeneration( true, maxMipCount );
                            compressor.Input.SetData( surface );
                            compressor.Compression.Format = compFormat;
                            compressor.Compression.SetBGRAPixelFormat();

                            compressor.Process( out var ddsContainer );
                            using( BinaryWriter writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                                replaceData = CreateAtex( texFile.Header.Format, ddsContainer, writer, convertToA8: ( texFile.Header.Format == TextureFormat.A8 ) );
                            }
                            ddsContainer.Dispose();
                        }
                    }
                }
                replaceData.localPath = path;
                // if there is already a replacement for the same file, delete the old file
                RemoveReplace( replacePath );
                // add/update dictionary
                if( !GamePathReplace.TryAdd( replacePath, replaceData ) ) {
                    return false;
                }
                // refresh preview texture if it exists
                RefreshPreview( replacePath );
                return true;
            }
            catch(Exception e ) {
                PluginLog.LogError(e, $"Error importing {fileLocation} into {replacePath}" );
            }
            return false;
        }

        public void RemoveReplace(string path ) {
            if( GamePathReplace.ContainsKey( path ) ) {
                if( GamePathReplace.TryRemove( path, out var oldValue ) ) {
                    File.Delete( oldValue.localPath );
                }
            }
        }

        public void RefreshPreview(string path ) {
            string paddedPath = path + '\u0000';
            if( PathToTex.ContainsKey( paddedPath ) ) {
                if( PathToTex.TryRemove( paddedPath, out var oldValue ) ) {
                    oldValue.Wrap?.Dispose();
                    LoadTexture( paddedPath );
                }
            }
        }

        public void Cleanup() {
            foreach(KeyValuePair<string, TexReplace> entry in GamePathReplace) {
                File.Delete( entry.Value.localPath );
            }
        }

        public static TexReplace CreateAtex(TextureFormat format, DDSContainer dds, BinaryWriter bw, bool convertToA8 = false ) {
            using( MemoryStream ms = new MemoryStream() ) {
                dds.Write( ms );
                using( BinaryReader br = new BinaryReader( ms ) ) {

                    TexReplace replaceData = new TexReplace();
                    replaceData.Format = format;
                    br.BaseStream.Seek( 12, SeekOrigin.Begin );
                    replaceData.Height = br.ReadInt32();
                    replaceData.Width = br.ReadInt32();
                    replaceData.Depth = br.ReadInt32();
                    replaceData.MipLevels = br.ReadInt32();

                    //br.BaseStream.Seek( 80, SeekOrigin.Begin );
                    //var textureFlags = br.ReadInt32();
                    //var texType = br.ReadInt32();
                    //var DDSInfo = IOUtil.GetDDSInfo( br, format, newWidth, newHeight, newMipCount );
                    //bw.Write( IOUtil.MakeType4DatHeader( format, DDSInfo.mipPartOffsets, DDSInfo.mipPartCounts, ( int )uncompressedLength, newMipCount, newWidth, newHeight ) );
                    //bw.Write( DDSInfo.compressedDDS.ToArray() );

                    bw.Write( IOUtil.MakeTextureInfoHeader( format, replaceData.Width, replaceData.Height, replaceData.MipLevels ).ToArray() ); // header is 80
                    br.BaseStream.Seek( 128, SeekOrigin.Begin );
                    var uncompressedLength = ms.Length - 128;
                    byte[] data = new byte[uncompressedLength];
                    br.Read( data, 0, ( int )uncompressedLength );
                    if( convertToA8 ) {
                        data = VFXTexture.CompressA8( data );
                    }
                    bw.Write( data );

                    return replaceData;
                }
            }
        }

        // ==== FOR DISPLAYING TEXTURES =======
        public VFXTexture GetTexture(string path ) {
            if( GamePathReplace.ContainsKey( path ) ) {
                return VFXTexture.LoadFromLocal( GamePathReplace[path].localPath );
            }
            else {
                return _plugin.PluginInterface.Data.GetFile<VFXTexture>( path );
            }
        }

        public void LoadTexture(string path ) {
            var _path = path.Trim( '\u0000' );
            if( PathToTex.ContainsKey( path ) )
                return;
            Task.Run( () => {
                var result = CreateTexture( _path, out var tex );
                if(result && tex.Wrap != null ) {
                    PathToTex.TryAdd( path, tex );
                }
            } );
        }

        public bool CreateTexture(string path, out TexData ret, bool loadImage = true )
        {
            var result = _plugin.PluginInterface.Data.FileExists( path );
            ret = new TexData();
            if( result ) {
                try {
                    VFXTexture texFile = GetTexture( path );
                    ret.Format = texFile.Header.Format;
                    ret.MipLevels = texFile.Header.MipLevels;
                    ret.Width = texFile.Header.Width;
                    ret.Height = texFile.Header.Height;
                    ret.Depth = texFile.Header.Depth;
                    ret.IsReplaced = texFile.Local;

                    if( !texFile.ValidFormat ) {
                        PluginLog.Log( $"Invalid format: {ret.Format} {path}" );
                        return false;
                    }

                    ret.Data = texFile.ImageData;
                    if( loadImage ) {
                        var texBind = _plugin.PluginInterface.UiBuilder.LoadImageRaw( ret.Data, texFile.Header.Width, texFile.Header.Height, 4 );
                        ret.Wrap = texBind;
                    }
                    return true;
                }
                catch( Exception e ) {
                    PluginLog.LogError( e, "Could not find tex:" + path );
                    return false;
                }
            }
            else {
                PluginLog.LogError( "Could not find tex:" + path );
                return false;
            }
        }
    }
}
