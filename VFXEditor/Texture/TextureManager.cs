using System;
using System.IO;
using System.Collections.Concurrent;

using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;

using Dalamud.Logging;

using VFXEditor.UI;

namespace VFXEditor.Texture {
    public struct PreviewTexture { // used for the texture previews
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
        public static TextureManager Manager { get; private set; }
        public ConcurrentDictionary<string, TextureReplace> PathToTextureReplace { get; private set; } = new(); // Keeps track of imported textures which replace existing ones

        private int TEX_ID = 0;
        private readonly ConcurrentDictionary<string, PreviewTexture> PathToTexturePreview = new(); // Keeps track of ImGui handles for previewed images

        public static void Initialize() {
            // Set paths manually since TexImpNet can be dumb sometimes
            var lib = TeximpNet.Unmanaged.FreeImageLibrary.Instance;

            var runtimeRoot = Path.Combine( Path.GetDirectoryName( Plugin.TemplateLocation ), "runtimes" );

            var _32bitPath = Path.Combine( runtimeRoot, "win-x64", "native" );
            var _64bitPath = Path.Combine( runtimeRoot, "win-x86", "native" );
            lib.Resolver.SetProbingPaths32( new string[] { _32bitPath } );
            lib.Resolver.SetProbingPaths64( new string[] { _64bitPath } );
            PluginLog.Log( $"TeximpNet paths: {_32bitPath} / {_64bitPath}" );

            ResetInstance();
        }

        public static void ResetInstance() {
            var oldInstance = Manager;
            Manager = new TextureManager();
            oldInstance?.DisposeInstance();
        }

        public static void Dispose() {
            Manager?.DisposeInstance();
            Manager = null;
        }

        // ======= INSTANCE ============

        public TextureManager() : base( "Imported Textures" ) {
        }

        public bool GetReplacePath( string gamePath, out string localPath ) {
            localPath = PathToTextureReplace.TryGetValue( gamePath, out var textureReplace ) ? textureReplace.LocalPath : null;
            return string.IsNullOrEmpty( localPath );
        }

        // import replacement texture from atex
        public void AddReplaceTexture( string localPath, string replacePath, int height, int width, int depth, int mips, TextureFormat format ) {
            if( !Plugin.DataManager.FileExists( replacePath ) ) {
                PluginLog.Error( $"{replacePath} does not exist" );
                return;
            }

            var path = Path.Combine( Configuration.Config.WriteLocation, "TexTemp" + ( TEX_ID++ ) + ".atex" );
            File.Copy( localPath, path, true );

            var replaceData = new TextureReplace {
                Height = height,
                Width = width,
                Depth = depth,
                MipLevels = mips,
                Format = format,
                LocalPath = path
            };

            PathToTextureReplace[replacePath] = replaceData;
        }

        // https://github.com/TexTools/xivModdingFramework/blob/872329d84c7b920fe2ac5e0b824d6ec5b68f4f57/xivModdingFramework/Textures/FileTypes/Tex.cs
        public bool AddReplaceTexture( string fileLocation, string replacePath ) {
            if( !Plugin.DataManager.FileExists( replacePath ) ) {
                PluginLog.Error( $"{replacePath} does not exist" );
                return false;
            }

            try {
                TextureReplace replaceData;
                var path = Path.Combine( Configuration.Config.WriteLocation, "TexTemp" + ( TEX_ID++ ) + ".atex" );

                if( Path.GetExtension( fileLocation ).ToLower() == ".dds" ) { // a .dds, use the format that the file is already in
                    var ddsFile = DDSFile.Read( fileLocation );
                    var format = VFXTexture.DXGItoTextureFormat( ddsFile.Format );
                    if( format == TextureFormat.Null )
                        return false;
                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateAtex( format, ddsFile, writer );
                    }
                    ddsFile.Dispose();
                }
                else if( Path.GetExtension( fileLocation ).ToLower() == ".atex" ) {
                    File.Copy( fileLocation, path, true );
                    var tex = VFXTexture.LoadFromLocal( fileLocation );
                    replaceData = new TextureReplace {
                        Height = tex.Header.Height,
                        Width = tex.Header.Width,
                        Depth = tex.Header.Depth,
                        MipLevels = tex.Header.MipLevels,
                        Format = tex.Header.Format,
                        LocalPath = path
                    };
                }
                else { //a .png file, convert it to the format currently being used by the existing game file
                    var texFile = Plugin.DataManager.GetFile<VFXTexture>( replacePath );

                    using var surface = Surface.LoadFromFile( fileLocation );
                    surface.FlipVertically();

                    using var compressor = new Compressor();
                    var compFormat = VFXTexture.TextureToCompressionFormat( texFile.Header.Format );
                    if( compFormat == CompressionFormat.ETC1 ) { // use ETC1 to signify "NULL" because I'm not going to be using it
                        return false;
                    }

                    compressor.Input.SetMipmapGeneration( true, texFile.Header.MipLevels ); // no limit on mipmaps. This is not true of stuff like UI textures (which are required to only have 1), but we don't have to worry about them
                    compressor.Input.SetData( surface );
                    compressor.Compression.Format = compFormat;
                    compressor.Compression.SetBGRAPixelFormat();
                    compressor.Process( out var ddsContainer );

                    using( var writer = new BinaryWriter( File.Open( path, FileMode.Create ) ) ) {
                        replaceData = CreateAtex( texFile.Header.Format, ddsContainer, writer, convertToA8: ( texFile.Header.Format == TextureFormat.A8 ) );
                    }
                    ddsContainer.Dispose();
                }
                // if there is already a replacement for the same file, delete the old file
                replaceData.LocalPath = path;
                RemoveReplaceTexture( replacePath );
                if( !PathToTextureReplace.TryAdd( replacePath, replaceData ) ) {
                    return false;
                }
                // refresh preview texture if it exists
                RefreshPreviewTexture( replacePath );
                return true;
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Error importing {fileLocation} into {replacePath}" );
            }
            return false;
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

            bw.Write( AtexHelper.CreateATEXHeader( format, replaceData.Width, replaceData.Height, replaceData.MipLevels ).ToArray() );
            br.BaseStream.Seek( 128, SeekOrigin.Begin );
            var uncompressedLength = ms.Length - 128;
            var data = new byte[uncompressedLength];
            br.Read( data, 0, ( int )uncompressedLength );
            if( convertToA8 ) { // scuffed way to handle png -> A8. Just load is as BGRA, then only keep the A channel
                data = VFXTexture.CompressA8( data );
            }
            bw.Write( data );

            return replaceData;
        }

        public VFXTexture GetRawTexture( string path ) {
            return PathToTextureReplace.TryGetValue( path, out var texturePreview ) ? VFXTexture.LoadFromLocal( texturePreview.LocalPath ) : Plugin.DataManager.GetFile<VFXTexture>( path );
        }

        public void LoadPreviewTexture( string path ) {
            var _path = path.Trim( '\u0000' );
            if( PathToTexturePreview.ContainsKey( path ) )
                return;
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

        public bool GetTexturePreview( string path, out PreviewTexture data ) => PathToTexturePreview.TryGetValue( path, out data );

        public bool CreatePreviewTexture( string path, out PreviewTexture ret, bool loadImage = true ) {
            var result = Plugin.DataManager.FileExists( path );
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

        private void DisposeInstance() {
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
