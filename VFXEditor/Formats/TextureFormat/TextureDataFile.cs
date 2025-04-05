using BCnEncoder.Decoder;
using Lumina.Data;
using Lumina.Data.Parsing.Tex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.FileBrowser;
using VfxEditor.Formats.TextureFormat.CustomTeximpNet;

namespace VfxEditor.Formats.TextureFormat {
    public enum Attribute : uint {
        DiscardPerFrame = 0x1,
        DiscardPerMap = 0x2,
        Managed = 0x4,
        UserManaged = 0x8,
        CpuRead = 0x10,
        LocationMain = 0x20,
        NoGpuRead = 0x40,
        AlignedSize = 0x80,
        EdgeCulling = 0x100,
        LocationOnion = 0x200,
        ReadWrite = 0x400,
        Immutable = 0x800,
        TextureRenderTarget = 0x100000,
        TextureDepthStencil = 0x200000,
        TextureType1D = 0x400000,
        TextureType2D = 0x800000,
        TextureType3D = 0x1000000,
        TextureType2DArray = 0x10000000,
        TextureTypeCube = 0x2000000,
        TextureTypeMask = 0x13C00000,
        TextureSwizzle = 0x4000000,
        TextureNoTiled = 0x8000000,
        TextureNoSwizzle = 0x80000000,
    }

    public enum TextureFormat {
        TypeShift = 0xC,
        TypeMask = 0xF000,
        ComponentShift = 0x8,
        ComponentMask = 0xF00,
        BppShift = 0x4,
        BppMask = 0xF0,
        EnumShift = 0x0,
        EnumMask = 0xF,
        TypeInteger = 0x1,
        TypeFloat = 0x2,
        TypeDxt = 0x3,
        TypeSpecial = 0x5,
        A8R8G8B8 = 0x1450,
        R8G8B8X8 = 0x1451,
        A8R8G8B82 = 0x1452,
        R4G4B4A4 = 0x1440,
        R5G5B5A1 = 0x1441,
        L8 = 0x1130,
        A8 = 0x1131,
        R32F = 0x2150,
        R32G32B32A32F = 0x2470,
        R16G16F = 0x2250,
        R16G16B16A16F = 0x2460,
        DXT1 = 0x3420,
        DXT3 = 0x3430,
        DXT5 = 0x3431,
        D16 = 0x4140,
        D24S8 = 0x4250,
        //todo: RGBA8 0x4401
        Null = 0x5100,
        Shadow16 = 0x5140,
        Shadow24 = 0x5150,
        BC5 = 0x6230,
        BC7 = 0x6432,
    }

    public class TextureDataFile : FileResource {
        [StructLayout( LayoutKind.Sequential, Pack = 4 )]
        public unsafe struct TexHeader {
            public Attribute Type;
            public TextureFormat Format;
            public ushort Width;
            public ushort Height;
            public ushort Depth;
            public byte MipLevelsCount;
            public byte ArraySize;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
            public uint[] LodOffset;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 13 )]
            public uint[] OffsetToSurface;

            public readonly DXGIFormat DXGIFormat => Format switch {
                TextureFormat.DXT1 => DXGIFormat.BC1_UNorm,
                TextureFormat.DXT5 => DXGIFormat.BC3_UNorm,
                TextureFormat.BC5 => DXGIFormat.BC5_UNorm,
                TextureFormat.BC7 => DXGIFormat.BC7_UNorm,
                TextureFormat.A8R8G8B8 => DXGIFormat.R8G8B8A8_UNorm,
                TextureFormat.R4G4B4A4 => DXGIFormat.B4G4R4A4_UNorm,
                TextureFormat.R5G5B5A1 => DXGIFormat.B5G5R5A1_UNorm,
                _ => DXGIFormat.R8G8B8A8_UNorm
            };
        };

        public TexHeader Header { get; private set; }
        public bool ValidFormat { get; private set; } = false;

        // Used for image previews
        public List<byte[]> Layers { get; private set; }
        public byte[] ImageData => Layers[0];

        private static int HeaderLength => Marshal.SizeOf<TexHeader>();
        private byte[] DdsData;

        public bool Local { get; private set; } = false; // was this loaded from the game using Lumina, or from a local ATEX?
        private byte[] LocalData;

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;

            var buffer = Reader.ReadBytes( HeaderLength );
            var handle = Marshal.AllocHGlobal( HeaderLength );
            Marshal.Copy( buffer, 0, handle, HeaderLength );
            Header = Marshal.PtrToStructure<TexHeader>( handle );
            Marshal.FreeHGlobal( handle );

            Layers = Convert( DataSpan[HeaderLength..].ToArray(), Header.Format, Header.Width, Header.Height, Header.Depth );
            ValidFormat = ImageData.Length > 0;
        }

        public void LoadFile( byte[] localData ) {
            LocalData = localData;
            using var ms = new MemoryStream( LocalData );
            using var reader = new BinaryReader( ms );

            Local = true;
            reader.BaseStream.Position = 0;

            var headerBuffer = reader.ReadBytes( HeaderLength );
            var handle = Marshal.AllocHGlobal( HeaderLength );
            Marshal.Copy( headerBuffer, 0, handle, HeaderLength );
            Header = Marshal.PtrToStructure<TexHeader>( handle );
            Marshal.FreeHGlobal( handle );

            DdsData = reader.ReadBytes( localData.Length - HeaderLength );

            Layers = Convert(
                DdsData,
                Header.Format,
                Header.Width,
                Header.Height,
                Header.ArraySize > 1 && Header.MipLevelsCount > 1 && Header.Type == Attribute.TextureType2DArray ? Header.ArraySize : Header.Depth
            );

            ValidFormat = ImageData.Length > 0;
        }

        public static TextureDataFile LoadFromLocal( string path ) {
            var tex = new TextureDataFile();
            tex.LoadFile( File.ReadAllBytes( path ) );
            return tex;
        }

        // converts various formats to A8R8G8B8
        public static List<byte[]> Convert( byte[] data, TextureFormat format, int width, int height, int layers ) {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            switch( format ) {
                case TextureFormat.DXT1:
                    DecompressDxt1( data, writer, width, height * layers );
                    break;
                case TextureFormat.DXT3:
                    DecompressDxt3( data, writer, width, height * layers );
                    break;
                case TextureFormat.DXT5:
                    DecompressDxt5( data, writer, width, height * layers );
                    break;
                case TextureFormat.A8R8G8B8:
                    writer.Write( data ); // already ok
                    break;
                case TextureFormat.R4G4B4A4:
                    Read4444( data, writer, width, height * layers );
                    break;
                case TextureFormat.R5G5B5A1:
                    Read5551( data, writer, width, height * layers );
                    break;
                case TextureFormat.A8:
                    DecompressA8( data, writer, width, height * layers );
                    break;
                case TextureFormat.BC5:
                    DecompressBc( data, writer, width, height * layers, BCnEncoder.Shared.CompressionFormat.Bc5 );
                    break;
                case TextureFormat.BC7:
                    DecompressBc( data, writer, width, height * layers, BCnEncoder.Shared.CompressionFormat.Bc7 );
                    break;
                default:
                    Dalamud.Log( $"Unknown format {format}" );
                    return [[]];
            }

            // TODO: R16G16B16A16F -> https://github.com/NotAdam/Lumina/blob/56a057f78ea8ee442f61f9dec3d4fb6fd109486a/src/Lumina/Data/Parsing/Tex/Buffers/R16G16B16A16FTextureBuffer.cs#L9

            var output = ms.ToArray();
            var size = width * height * 4;

            var res = new List<byte[]>();
            for( var i = 0; i < layers; i++ ) {
                var offset = size * i;
                var layer = new byte[size];
                var remaining = output.Length - offset;
                if( remaining <= 0 ) continue;
                Array.Copy( output, offset, layer, 0, Math.Min( remaining, size ) );
                res.Add( BgraToRgba( layer ) );
            }
            return res;
        }

        public static TextureFormat DXGItoTextureFormat( DXGIFormat format ) {
            return format switch {
                DXGIFormat.A8_UNorm => TextureFormat.A8,
                DXGIFormat.BC1_UNorm => TextureFormat.DXT1,
                DXGIFormat.BC2_UNorm => TextureFormat.DXT3,
                DXGIFormat.BC3_UNorm => TextureFormat.DXT5,
                DXGIFormat.BC7_UNorm => TextureFormat.BC7,
                DXGIFormat.B8G8R8A8_UNorm => TextureFormat.A8R8G8B8,
                DXGIFormat.B4G4R4A4_UNorm => TextureFormat.R4G4B4A4,
                DXGIFormat.B5G5R5A1_UNorm => TextureFormat.R5G5B5A1,
                _ => TextureFormat.Null,
            };
        }

        public static CompressionFormat TextureToCompressionFormat( TextureFormat format ) {
            return format switch {
                TextureFormat.DXT1 => CompressionFormat.BC1a,
                TextureFormat.DXT3 => CompressionFormat.BC2,
                TextureFormat.DXT5 => CompressionFormat.BC3,
                TextureFormat.BC5 => CompressionFormat.BC5,
                TextureFormat.BC7 => CompressionFormat.BC7,
                TextureFormat.A8R8G8B8 or TextureFormat.R4G4B4A4 or TextureFormat.A8 or TextureFormat.R5G5B5A1 => CompressionFormat.BGRA,
                _ => CompressionFormat.ETC1,
            }; ;
        }

        public static byte[] CompressA8( byte[] data ) {
            // r g b A r g b A, ... only take the A part
            var ret = new byte[data.Length / 4];
            for( var i = 0; i < ret.Length; i++ ) {
                ret[i] = data[i * 4 + 3];
            }
            return ret;
        }

        private static void DecompressA8( byte[] data, BinaryWriter writer, int width, int height ) {
            for( var i = 0; i < width * height; i++ ) {
                writer.Write( ( byte )0xFF );
                writer.Write( ( byte )0xFF );
                writer.Write( ( byte )0xFF );
                writer.Write( data[i] );
            }
        }

        private static void DecompressDxt1( byte[] data, BinaryWriter writer, int width, int height ) {
            writer.Write( Squish.DecompressImage( data, width, height, SquishOptions.DXT1 ) );
        }

        private static void DecompressDxt3( byte[] data, BinaryWriter writer, int width, int height ) {
            writer.Write( Squish.DecompressImage( data, width, height, SquishOptions.DXT3 ) );
        }

        private static void DecompressDxt5( byte[] data, BinaryWriter writer, int width, int height ) {
            writer.Write( Squish.DecompressImage( data, width, height, SquishOptions.DXT5 ) );
        }

        private static void DecompressBc( byte[] data, BinaryWriter writer, int width, int height, BCnEncoder.Shared.CompressionFormat format ) {
            var decoder = new BcDecoder();
            var output = decoder.DecodeRaw2D( data, width, height, format ).ToArray();

            for( var i = 0; i < height; i++ ) {
                for( var j = 0; j < width; j++ ) {
                    var pixel = output[i, j];
                    writer.Write( pixel.b );
                    writer.Write( pixel.g );
                    writer.Write( pixel.r );
                    writer.Write( pixel.a );
                }
            }
        }

        private static void Read4444( byte[] src, BinaryWriter writer, int width, int height ) {
            using var ms = new MemoryStream( src );
            using var reader = new BinaryReader( ms );

            for( var y = 0; y < height; y++ ) {
                for( var x = 0; x < width; x++ ) {
                    var pixel = reader.ReadUInt16() & 0xFFFF;
                    var blue = ( pixel & 0xF ) * 16;
                    var green = ( ( pixel & 0xF0 ) >> 4 ) * 16;
                    var red = ( ( pixel & 0xF00 ) >> 8 ) * 16;
                    var alpha = ( ( pixel & 0xF000 ) >> 12 ) * 16;

                    writer.Write( ( byte )blue );
                    writer.Write( ( byte )green );
                    writer.Write( ( byte )red );
                    writer.Write( ( byte )alpha );
                }
            }
        }

        private static void Read5551( byte[] src, BinaryWriter writer, int width, int height ) {
            using var ms = new MemoryStream( src );
            using var reader = new BinaryReader( ms );

            for( var y = 0; y < height; y++ ) {
                for( var x = 0; x < width; x++ ) {
                    var pixel = reader.ReadUInt16() & 0xFFFF;
                    var blue = ( pixel & 0x001F ) * 8;
                    var green = ( ( pixel & 0x03E0 ) >> 5 ) * 8;
                    var red = ( ( pixel & 0x7C00 ) >> 10 ) * 8;
                    var alpha = ( ( pixel & 0x8000 ) >> 15 ) * 255;

                    writer.Write( ( byte )blue );
                    writer.Write( ( byte )green );
                    writer.Write( ( byte )red );
                    writer.Write( ( byte )alpha );
                }
            }
        }

        private static byte[] BgraToRgba( byte[] data ) {
            var ret = new byte[data.Length];
            for( var i = 0; i < data.Length / 4; i++ ) {
                var idx = i * 4;
                ret[idx + 0] = data[idx + 2];
                ret[idx + 1] = data[idx + 1];
                ret[idx + 2] = data[idx + 0];
                ret[idx + 3] = data[idx + 3];
            }
            return ret;
        }

        // ==================

        public byte[] GetAllData() => Local ? LocalData : Data;

        public byte[] GetDdsData() => Local ? DdsData : DataSpan[HeaderLength..].ToArray();

        public Surface GetPngData( out nint pin ) {
            var data = new RGBAQuad[Header.Height * Header.Width];
            for( var i = 0; i < Header.Height; i++ ) {
                for( var j = 0; j < Header.Width; j++ ) {
                    var idx = i * Header.Width + j;
                    data[idx] = new RGBAQuad( ImageData[idx * 4], ImageData[idx * 4 + 1], ImageData[idx * 4 + 2], ImageData[idx * 4 + 3] );
                }
            }
            pin = MemoryHelper.PinObject( data );
            return Surface.LoadFromRawData( pin, Header.Width, Header.Height, Header.Width * 4, false, true );
        }

        public void SaveAsPng( string path ) {
            var surface = GetPngData( out var pin );
            if( surface == null ) return;

            surface.SaveToFile( ImageFormat.PNG, path );
            surface.Dispose();
            MemoryHelper.UnpinObject( pin );
        }

        public void SaveAsDds( string path ) {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            using var buffer = new StreamTransferBuffer();
            CustomDDSFile.WriteHeader( ms, buffer, TextureDimension.Two,
                Header.DXGIFormat, Header.Width, Header.Height, Header.Depth, Header.ArraySize, Header.MipLevelsCount, DDSFlags.None );

            writer.BaseStream.Position = ms.Length;
            writer.Write( GetDdsData() );
            File.WriteAllBytes( path, ms.ToArray() );
        }

        public void SavePngDialog() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( bool ok, string res ) => {
                if( !ok ) return;
                SaveAsPng( res );
            } );
        }

        public void SaveDdsDialog() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( bool ok, string res ) => {
                if( !ok ) return;
                SaveAsDds( res );
            } );
        }

        public void SaveTexDialog( string ext ) {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", $".{ext}", "ExportedTexture", ext, ( bool ok, string res ) => {
                if( !ok ) return;
                File.WriteAllBytes( res, GetAllData() );
            } );
        }
    }
}
