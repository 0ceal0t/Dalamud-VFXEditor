using Dalamud.Logging;
using System;
using System.IO;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.TextureFormat.CustomTeximpNet;
using VfxEditor.TextureFormat.Textures;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat {
    public partial class TextureManager {
        private enum PostConversion {
            None,
            A8,
            R4444
        }

        // https://github.com/TexTools/xivModdingFramework/blob/872329d84c7b920fe2ac5e0b824d6ec5b68f4f57/xivModdingFramework/Textures/FileTypes/Tex.cs
        public bool ImportTexture( string importPath, string gamePath, ushort pngMip = 9, TextureFormat pngFormat = TextureFormat.DXT5 ) {
            gamePath = gamePath.Trim( '\0' );

            try {
                TextureReplace replaceData;
                var gameFileExtension = gamePath.Split( '.' )[^1].Trim( '\0' );
                var localFileExtension = Path.GetExtension( importPath ).ToLower();
                var localPath = Path.Combine( Plugin.Configuration.WriteLocation, "TexTemp" + ( TEX_ID++ ) + "." + gameFileExtension );

                if( localFileExtension == ".dds" ) { // a .dds, use the format that the file is already in
                    var ddsFile = DDSFile.Read( importPath );
                    var format = TextureFile.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null ) return false;

                    using( var writer = new BinaryWriter( File.Open( localPath, FileMode.Create ) ) ) {
                        replaceData = CreateTextureReplace( format, localPath, gamePath, File.ReadAllBytes( importPath ), writer );
                    }
                    ddsFile.Dispose();
                }
                else if( localFileExtension == ".atex" || localFileExtension == ".tex" ) {
                    File.Copy( importPath, localPath, true );
                    var tex = TextureFile.LoadFromLocal( importPath );

                    replaceData = new TextureReplace( localPath, gamePath, tex.Header.Height, tex.Header.Width, tex.Header.Depth, tex.Header.MipLevels, tex.Header.Format );
                }
                else { // .png
                    using var surface = Surface.LoadFromFile( importPath );
                    surface.FlipVertically();

                    using var compressor = new Compressor();
                    var compFormat = TextureFile.TextureToCompressionFormat( pngFormat );
                    // use ETC1 to signify "NULL" because I'm not going to be using it
                    if( compFormat == CompressionFormat.ETC1 ) return false;

                    // Ui elements are required to only have 1 mip level
                    var maxMips = gamePath.StartsWith( "ui/" ) ? 1 : pngMip;
                    compressor.Input.SetMipmapGeneration( true, maxMips );
                    compressor.Input.SetData( surface );
                    compressor.Compression.Format = compFormat;
                    compressor.Compression.SetBGRAPixelFormat();
                    compressor.Process( out var ddsContainer );

                    using var ms = new MemoryStream();

                    // lord have mercy
                    CustomDDSFile.Write( ms, ddsContainer.MipChains, ddsContainer.Format, ddsContainer.Dimension );

                    var ddsData = ms.ToArray();

                    using( var writer = new BinaryWriter( File.Open( localPath, FileMode.Create ) ) ) {
                        var postConversion = pngFormat switch {
                            TextureFormat.A8 => PostConversion.A8,
                            TextureFormat.R4G4B4A4 => PostConversion.R4444,
                            _ => PostConversion.None
                        };

                        replaceData = CreateTextureReplace( pngFormat, localPath, gamePath, ddsData, writer, postConversion );
                    }
                    ddsContainer.Dispose();
                }

                return ReplaceAndRefreshTexture( replaceData, gamePath );
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Error importing {importPath} into {gamePath}" );
            }
            return false;
        }

        private bool ReplaceAndRefreshTexture( TextureReplace data, string gamePath ) {
            // if there is already a replacement for the same file, delete the old file
            RemoveReplaceTexture( gamePath );
            if( !PathToTextureReplace.TryAdd( gamePath, data ) ) return false;

            // refresh preview texture if it exists
            RefreshPreviewTexture( gamePath );
            return true;
        }

        private void RemoveReplaceTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( PathToTextureReplace.ContainsKey( gamePath ) && PathToTextureReplace.TryRemove( gamePath, out var oldValue ) ) {
                File.Delete( oldValue.LocalPath );
                Plugin.CleanupExport( oldValue );
            }
        }

        private void RefreshPreviewTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( PathToTexturePreview.ContainsKey( gamePath ) ) {
                if( PathToTexturePreview.TryRemove( gamePath, out var oldValue ) ) {
                    oldValue.Wrap?.Dispose();
                }
            }
        }

        public PreviewTexture GetPreviewTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( string.IsNullOrEmpty( gamePath ) || !gamePath.Contains( '/' ) ) return null;

            if( PathToTexturePreview.TryGetValue( gamePath, out var data ) ) return data;

            // Doesn't exist yet, try to load
            var texture = CreatePreviewTexture( gamePath );
            if( texture != null && texture.Wrap != null ) {
                PathToTexturePreview.TryAdd( gamePath, texture );
                return texture;
            }

            return null;
        }

        private PreviewTexture CreatePreviewTexture( string gamePath, bool loadImage = true ) {
            var result = Plugin.DataManager.FileExists( gamePath ) || PathToTextureReplace.ContainsKey( gamePath );

            if( !result ) return null;

            try {
                var texFile = GetRawTexture( gamePath );

                if( !texFile.ValidFormat ) {
                    PluginLog.Error( $"Invalid format: {texFile.Header.Format} {gamePath}" );
                    return null;
                }

                return new PreviewTexture( texFile, gamePath, loadImage );
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not find tex: {gamePath}" );
                return null;
            }
        }

        // https://github.com/TexTools/xivModdingFramework/blob/master/xivModdingFramework/Textures/FileTypes/Tex.cs#L1002
        private static TextureReplace CreateTextureReplace( TextureFormat format, string localPath, string gamePath, byte[] dds, BinaryWriter writer, PostConversion post = PostConversion.None ) {
            // Get DDS info
            using var ddsMs = new MemoryStream( dds );
            using var ddsReader = new BinaryReader( ddsMs );
            ddsReader.BaseStream.Seek( 12, SeekOrigin.Begin );
            var height = ddsReader.ReadInt32();
            var width = ddsReader.ReadInt32();
            var pitch = ddsReader.ReadInt32();
            var depth = ddsReader.ReadInt32();
            var mipLevels = ddsReader.ReadInt32();

            writer.Write( TextureUtils.CreateTextureHeader( format, width, height, mipLevels ).ToArray() );

            // Add DDS data
            ddsReader.BaseStream.Seek( 128, SeekOrigin.Begin );
            var uncompressedLength = ddsMs.Length - 128;
            var ddsData = new byte[uncompressedLength];
            ddsReader.Read( ddsData, 0, ( int )uncompressedLength );
            // scuffed way to handle png -> A8. Just load is as BGRA, then only keep the A channel
            // Not currently used
            if( post == PostConversion.A8 ) ddsData = TextureFile.CompressA8( ddsData );

            writer.Write( ddsData );

            return new TextureReplace( localPath, gamePath, height, width, depth, mipLevels, format );
        }

        private TextureFile GetRawTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            return PathToTextureReplace.TryGetValue( gamePath, out var texturePreview ) ?
                TextureFile.LoadFromLocal( texturePreview.LocalPath ) :
                Plugin.DataManager.GetFile<TextureFile>( gamePath );
        }
    }
}
