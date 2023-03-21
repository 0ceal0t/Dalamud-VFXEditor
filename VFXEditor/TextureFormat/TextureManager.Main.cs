using Dalamud.Logging;
using System;
using System.IO;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.TextureFormat.CustomTeximpNet;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat {
    public partial class TextureManager {
        // https://github.com/TexTools/xivModdingFramework/blob/872329d84c7b920fe2ac5e0b824d6ec5b68f4f57/xivModdingFramework/Textures/FileTypes/Tex.cs
        public bool ImportTexture( string localPath, string replacePath, ushort pngMip = 9, TextureFormat pngFormat = TextureFormat.DXT5 ) {
            try {
                TextureReplace replaceData;
                var path = Path.Combine( Plugin.Configuration.WriteLocation, "TexTemp" + ( TEX_ID++ ) + ".atex" );

                if( Path.GetExtension( localPath ).ToLower() == ".dds" ) { // a .dds, use the format that the file is already in
                    var ddsFile = DDSFile.Read( localPath );
                    var format = AtexFile.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null ) return false;

                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateAtex( format, File.ReadAllBytes( localPath ), writer );
                    }
                    ddsFile.Dispose();
                }
                else if( Path.GetExtension( localPath ).ToLower() == ".atex" ) {
                    File.Copy( localPath, path, true );
                    var tex = AtexFile.LoadFromLocal( localPath );
                    replaceData = new TextureReplace {
                        Height = tex.Header.Height,
                        Width = tex.Header.Width,
                        Depth = tex.Header.Depth,
                        MipLevels = tex.Header.MipLevels,
                        Format = tex.Header.Format,
                        LocalPath = path
                    };
                }
                else { // .png
                    using var surface = Surface.LoadFromFile( localPath );
                    surface.FlipVertically();

                    using var compressor = new Compressor();
                    var compFormat = AtexFile.TextureToCompressionFormat( pngFormat );
                    // use ETC1 to signify "NULL" because I'm not going to be using it
                    if( compFormat == CompressionFormat.ETC1 ) return false;

                    compressor.Input.SetMipmapGeneration( true, pngMip ); // no limit on mipmaps. This is not true of stuff like UI textures (which are required to only have 1), but we don't have to worry about them
                    compressor.Input.SetData( surface );
                    compressor.Compression.Format = compFormat;
                    compressor.Compression.SetBGRAPixelFormat();
                    compressor.Process( out var ddsContainer );

                    using var ms = new MemoryStream();

                    // lord have mercy
                    CustomDDSFile.Write( ms, ddsContainer.MipChains, ddsContainer.Format, ddsContainer.Dimension );

                    var ddsData = ms.ToArray();

                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateAtex( pngFormat, ddsData, writer, convertToA8: ( pngFormat == TextureFormat.A8 ) );
                    }
                    ddsContainer.Dispose();
                }
                replaceData.LocalPath = path;
                return ReplaceAndRefreshTexture( replaceData, replacePath );
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Error importing {localPath} into {replacePath}" );
            }
            return false;
        }

        private bool ReplaceAndRefreshTexture( TextureReplace data, string path ) {
            // if there is already a replacement for the same file, delete the old file
            RemoveReplaceTexture( path );
            if( !PathToTextureReplace.TryAdd( path, data ) ) return false;

            // refresh preview texture if it exists
            RefreshPreviewTexture( path );
            return true;
        }

        public void RemoveReplaceTexture( string path ) {
            if( PathToTextureReplace.ContainsKey( path ) && PathToTextureReplace.TryRemove( path, out var oldValue ) ) File.Delete( oldValue.LocalPath );
        }

        private static TextureReplace CreateAtex( TextureFormat format, byte[] ddsData, BinaryWriter bw, bool convertToA8 = false ) {
            using var ms = new MemoryStream( ddsData );
            using var br = new BinaryReader( ms );
            var replaceData = new TextureReplace {
                Format = format
            };
            br.BaseStream.Seek( 12, SeekOrigin.Begin );
            replaceData.Height = br.ReadInt32();
            replaceData.Width = br.ReadInt32();
            var pitch = br.ReadInt32();
            replaceData.Depth = br.ReadInt32();
            replaceData.MipLevels = br.ReadInt32();

            bw.Write( AtexUtils.CreateAtexHeader( format, replaceData.Width, replaceData.Height, replaceData.MipLevels ).ToArray() );
            br.BaseStream.Seek( 128, SeekOrigin.Begin );
            var uncompressedLength = ms.Length - 128;
            var data = new byte[uncompressedLength];
            br.Read( data, 0, ( int )uncompressedLength );
            if( convertToA8 ) { // scuffed way to handle png -> A8. Just load is as BGRA, then only keep the A channel
                data = AtexFile.CompressA8( data );
            }
            bw.Write( data );

            return replaceData;
        }

        public AtexFile GetRawTexture( string path ) {
            return PathToTextureReplace.TryGetValue( path, out var texturePreview ) ? AtexFile.LoadFromLocal( texturePreview.LocalPath ) : Plugin.DataManager.GetFile<AtexFile>( path );
        }

        public void LoadPreviewTexture( string path ) {
            var _path = path.Trim( '\u0000' );
            if( PathToTexturePreview.ContainsKey( path ) ) return; // Already loaded

            var result = CreatePreviewTexture( _path, out var tex );
            if( result && tex.Wrap != null ) {
                PathToTexturePreview.TryAdd( path, tex );
            }
        }

        public void RefreshPreviewTexture( string path ) {
            var paddedPath = path + '\u0000';
            if( PathToTexturePreview.ContainsKey( paddedPath ) ) {
                if( PathToTexturePreview.TryRemove( paddedPath, out var oldValue ) ) {
                    oldValue.Wrap?.Dispose();
                }
            }
            LoadPreviewTexture( paddedPath );
        }

        public bool GetPreviewTexture( string path, out PreviewTexture data ) => PathToTexturePreview.TryGetValue( path, out data );

        public bool CreatePreviewTexture( string path, out PreviewTexture ret, bool loadImage = true ) {
            var result = Plugin.DataManager.FileExists( path ) || PathToTextureReplace.ContainsKey( path );
            ret = new PreviewTexture();
            if( result ) {
                try {
                    var texFile = GetRawTexture( path );
                    ret.Format = texFile.Header.Format;
                    ret.MipLevels = texFile.Header.MipLevels;
                    ret.Width = texFile.Header.Width;
                    ret.Height = texFile.Header.Height;
                    ret.Depth = texFile.Header.Depth;
                    ret.IsReplaced = texFile.Local;

                    if( !texFile.ValidFormat ) {
                        PluginLog.Error( $"Invalid format: {ret.Format} {path}" );
                        return false;
                    }

                    if( loadImage ) {
                        var texBind = Plugin.PluginInterface.UiBuilder.LoadImageRaw( texFile.ImageData, texFile.Header.Width, texFile.Header.Height, 4 );
                        ret.Wrap = texBind;
                    }
                    return true;
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not find tex: " + path );
                    return false;
                }
            }
            else {
                PluginLog.Error( "Could not find tex: " + path );
                return false;
            }
        }
    }
}
