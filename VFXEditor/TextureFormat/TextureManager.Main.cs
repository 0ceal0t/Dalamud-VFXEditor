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
        private enum PostConversion {
            None,
            A8,
            R4444
        }

        // https://github.com/TexTools/xivModdingFramework/blob/872329d84c7b920fe2ac5e0b824d6ec5b68f4f57/xivModdingFramework/Textures/FileTypes/Tex.cs
        public bool ImportTexture( string localPath, string gamePath, ushort pngMip = 9, TextureFormat pngFormat = TextureFormat.DXT5 ) {
            gamePath = gamePath.Trim( '\0' );

            try {
                TextureReplace replaceData;
                var gameFileExtension = gamePath.Split( '.' )[^1].Trim( '\0' );
                var localFileExtension = Path.GetExtension( localPath ).ToLower();
                var path = Path.Combine( Plugin.Configuration.WriteLocation, "TexTemp" + ( TEX_ID++ ) + "." + gameFileExtension );

                if( localFileExtension == ".dds" ) { // a .dds, use the format that the file is already in
                    var ddsFile = DDSFile.Read( localPath );
                    var format = TextureFile.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null ) return false;

                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateTexture( format, File.ReadAllBytes( localPath ), writer );
                    }
                    ddsFile.Dispose();
                }
                else if( localFileExtension == ".atex" || localFileExtension == ".tex" ) {
                    File.Copy( localPath, path, true );
                    var tex = TextureFile.LoadFromLocal( localPath );
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

                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        var postConversion = pngFormat switch {
                            TextureFormat.A8 => PostConversion.A8,
                            TextureFormat.R4G4B4A4 => PostConversion.R4444,
                            _ => PostConversion.None
                        };

                        replaceData = CreateTexture( pngFormat, ddsData, writer, postConversion );
                    }
                    ddsContainer.Dispose();
                }
                replaceData.LocalPath = path;
                return ReplaceAndRefreshTexture( replaceData, gamePath );
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Error importing {localPath} into {gamePath}" );
            }
            return false;
        }

        private bool ReplaceAndRefreshTexture( TextureReplace data, string gamePath ) {
            gamePath = gamePath.Trim( '\0' );

            // if there is already a replacement for the same file, delete the old file
            RemoveReplaceTexture( gamePath );
            if( !PathToTextureReplace.TryAdd( gamePath, data ) ) return false;

            // refresh preview texture if it exists
            RefreshPreviewTexture( gamePath );
            return true;
        }

        public void RemoveReplaceTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( PathToTextureReplace.ContainsKey( gamePath ) && PathToTextureReplace.TryRemove( gamePath, out var oldValue ) ) File.Delete( oldValue.LocalPath );
        }

        // https://github.com/TexTools/xivModdingFramework/blob/master/xivModdingFramework/Textures/FileTypes/Tex.cs#L1002
        private static TextureReplace CreateTexture( TextureFormat format, byte[] dds, BinaryWriter writer, PostConversion post = PostConversion.None ) {
            // Get DDS info
            using var ddsMs = new MemoryStream( dds );
            using var ddsReader = new BinaryReader( ddsMs );
            var replaceData = new TextureReplace {
                Format = format
            };
            ddsReader.BaseStream.Seek( 12, SeekOrigin.Begin );
            replaceData.Height = ddsReader.ReadInt32();
            replaceData.Width = ddsReader.ReadInt32();
            var pitch = ddsReader.ReadInt32();
            replaceData.Depth = ddsReader.ReadInt32();
            replaceData.MipLevels = ddsReader.ReadInt32();

            writer.Write( TextureUtils.CreateTextureHeader( format, replaceData.Width, replaceData.Height, replaceData.MipLevels ).ToArray() );

            // Add DDS data
            ddsReader.BaseStream.Seek( 128, SeekOrigin.Begin );
            var uncompressedLength = ddsMs.Length - 128;
            var ddsData = new byte[uncompressedLength];
            ddsReader.Read( ddsData, 0, ( int )uncompressedLength );
            // scuffed way to handle png -> A8. Just load is as BGRA, then only keep the A channel
            // Not currently used
            if( post == PostConversion.A8 ) ddsData = TextureFile.CompressA8( ddsData );

            writer.Write( ddsData );

            return replaceData;
        }

        public TextureFile GetRawTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            return PathToTextureReplace.TryGetValue( gamePath, out var texturePreview ) ? TextureFile.LoadFromLocal( texturePreview.LocalPath ) : Plugin.DataManager.GetFile<TextureFile>( gamePath );
        }

        public void LoadPreviewTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( PathToTexturePreview.ContainsKey( gamePath ) ) return; // Already loaded

            var result = CreatePreviewTexture( gamePath, out var tex );
            if( result && tex.Wrap != null ) {
                PathToTexturePreview.TryAdd( gamePath, tex );
            }
        }

        public void RefreshPreviewTexture( string gamePath ) {
            gamePath = gamePath.Trim( '\0' );
            if( PathToTexturePreview.ContainsKey( gamePath ) ) {
                if( PathToTexturePreview.TryRemove( gamePath, out var oldValue ) ) {
                    oldValue.Wrap?.Dispose();
                }
            }
            LoadPreviewTexture( gamePath );
        }

        public bool GetPreviewTexture( string gamePath, out PreviewTexture data ) => PathToTexturePreview.TryGetValue( gamePath.Trim( '\0' ), out data );

        public bool CreatePreviewTexture( string gamePath, out PreviewTexture ret, bool loadImage = true ) {
            gamePath = gamePath.Trim( '\0' );
            var result = Plugin.DataManager.FileExists( gamePath ) || PathToTextureReplace.ContainsKey( gamePath );
            ret = new PreviewTexture();
            if( result ) {
                try {
                    var texFile = GetRawTexture( gamePath );
                    ret.Format = texFile.Header.Format;
                    ret.MipLevels = texFile.Header.MipLevels;
                    ret.Width = texFile.Header.Width;
                    ret.Height = texFile.Header.Height;
                    ret.Depth = texFile.Header.Depth;
                    ret.IsReplaced = texFile.Local;

                    if( !texFile.ValidFormat ) {
                        PluginLog.Error( $"Invalid format: {ret.Format} {gamePath}" );
                        return false;
                    }

                    if( loadImage ) {
                        var texBind = Plugin.PluginInterface.UiBuilder.LoadImageRaw( texFile.ImageData, texFile.Header.Width, texFile.Header.Height, 4 );
                        ret.Wrap = texBind;
                    }
                    return true;
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Could not find tex: " + gamePath );
                    return false;
                }
            }
            else {
                PluginLog.Error( "Could not find tex: " + gamePath );
                return false;
            }
        }
    }
}
