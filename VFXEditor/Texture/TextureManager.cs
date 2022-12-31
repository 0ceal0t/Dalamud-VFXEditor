using Dalamud.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.Dialogs;
using VfxEditor.Utils;

namespace VfxEditor.Texture {
    public struct PreviewTexture { // ImGui texture previews
        public ushort Height;
        public ushort Width;
        public ushort MipLevels;
        public ushort Depth;
        public bool IsReplaced;
        public TextureFormat Format;
        public ImGuiScene.TextureWrap Wrap;
    }

    public struct TextureReplace {
        public string LocalPath;
        public int Height;
        public int Width;
        public int Depth;
        public int MipLevels;
        public TextureFormat Format;
    }

    public partial class TextureManager : GenericDialog {
        public static readonly string PenumbraPath = "Tex";

        private int TEX_ID = 0;
        private readonly ConcurrentDictionary<string, TextureReplace> PathToTextureReplace = new(); // Keeps track of imported textures which replace existing ones
        private readonly ConcurrentDictionary<string, PreviewTexture> PathToTexturePreview = new(); // Keeps track of ImGui handles for previewed images

        public static void Setup() {
            // Set paths manually since TexImpNet can be dumb sometimes
            // Using the 32-bit version in all cases because net6, I guess
            var runtimeRoot = Path.Combine( Plugin.RootLocation, "runtimes" );

            // ==============

            var freeImgLib = TeximpNet.Unmanaged.FreeImageLibrary.Instance;
            var _32bitPath = Path.Combine( runtimeRoot, "win-x64", "native", "FreeImage.dll" );
            var _64bitPath = Path.Combine( runtimeRoot, "win-x86", "native", "FreeImage.dll" );
            freeImgLib.Resolver.SetOverrideLibraryName32( _32bitPath );
            freeImgLib.Resolver.SetOverrideLibraryName64( _32bitPath );
            PluginLog.Log( $"FreeImage TeximpNet paths: {_32bitPath} / {_64bitPath}" );
            PluginLog.Log( $"FreeImage Default name: {freeImgLib.DefaultLibraryName} Library loaded: {freeImgLib.IsLibraryLoaded}" );
            freeImgLib.LoadLibrary();
            PluginLog.Log( $"FreeImage Library path: {freeImgLib.LibraryPath} Library loaded: {freeImgLib.IsLibraryLoaded}" );

            // ===============

            var nvtLib = TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance;
            var nv_32bitPath = Path.Combine( runtimeRoot, "win-x64", "native", "nvtt.dll" );
            var nv_64bitPath = Path.Combine( runtimeRoot, "win-x86", "native", "nvtt.dll" );
            nvtLib.Resolver.SetOverrideLibraryName32( nv_32bitPath );
            nvtLib.Resolver.SetOverrideLibraryName64( nv_32bitPath );
            PluginLog.Log( $"NVT TeximpNet paths: {nv_32bitPath} / {nv_64bitPath}" );
            PluginLog.Log( $"NVT Default name: {nvtLib.DefaultLibraryName} Library loaded: {nvtLib.IsLibraryLoaded}" );
            nvtLib.LoadLibrary();
            PluginLog.Log( $"NVT Library path: {nvtLib.LibraryPath} Library loaded: {nvtLib.IsLibraryLoaded}" );
        }

        public static void BreakDown() {
            TeximpNet.Unmanaged.FreeImageLibrary.Instance.FreeLibrary();
            TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance.FreeLibrary();
            PluginLog.Log( $"FreeImage Library loaded: {TeximpNet.Unmanaged.FreeImageLibrary.Instance.IsLibraryLoaded}" );
            PluginLog.Log( $"NVTT Library loaded: { TeximpNet.Unmanaged.NvTextureToolsLibrary.Instance.IsLibraryLoaded}" );
        }

        public TextureManager() : base( "Imported Textures", false, 600, 400 ) { }

        public bool GetReplacePath( string gamePath, out string localPath ) {
            localPath = PathToTextureReplace.TryGetValue( gamePath, out var textureReplace ) ? textureReplace.LocalPath : null;
            return !string.IsNullOrEmpty( localPath );
        }

        // import replacement texture from atex
        public bool ImportTexture( string localPath, string replacePath, int height, int width, int depth, int mips, TextureFormat format ) {
            var path = Path.Combine( Plugin.Configuration.WriteLocation, "TexTemp" + ( TEX_ID++ ) + ".atex" );
            File.Copy( localPath, path, true );

            var replaceData = new TextureReplace {
                Height = height,
                Width = width,
                Depth = depth,
                MipLevels = mips,
                Format = format,
                LocalPath = path
            };
            return ReplaceAndRefreshTexture( replaceData, replacePath );
        }

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
                        replaceData = CreateAtex( format, ddsFile, writer );
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
                else {
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

                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateAtex( pngFormat, ddsContainer, writer, convertToA8: ( pngFormat == TextureFormat.A8 ) );
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

        private bool ReplaceAndRefreshTexture(TextureReplace data, string path) {
            // if there is already a replacement for the same file, delete the old file
            RemoveReplaceTexture( path );
            if( !PathToTextureReplace.TryAdd( path, data ) ) return false;

            // refresh preview texture if it exists
            RefreshPreviewTexture( path );
            return true;
        }

        public void RemoveReplaceTexture( string path ) {
            if( PathToTextureReplace.ContainsKey( path ) ) {
                if( PathToTextureReplace.TryRemove( path, out var oldValue ) ) {
                    File.Delete( oldValue.LocalPath );
                }
            }
        }

        private static TextureReplace CreateAtex( TextureFormat format, DDSContainer dds, BinaryWriter bw, bool convertToA8 = false ) {
            using var ms = new MemoryStream();
            dds.Write( ms );
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
            var result = Plugin.DataManager.FileExists( path ) || PathToTextureReplace.ContainsKey( path) ;
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

        public void Dispose() {
            foreach( var entry in PathToTexturePreview ) {
                entry.Value.Wrap?.Dispose();
            }
            foreach( var entry in PathToTextureReplace ) {
                File.Delete( entry.Value.LocalPath );
            }
            PathToTexturePreview.Clear();
            PathToTextureReplace.Clear();
        }
    }
}
