using Lumina.Data.Parsing.Tex;
using Lumina.Extensions;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.Utils;

namespace VfxEditor.TextureFormat {
    public class TextureFile : Lumina.Data.FileResource {
        [StructLayout( LayoutKind.Sequential )]
        public unsafe struct TexHeader {
            public Attribute Type;
            public TextureFormat Format;
            public ushort Width;
            public ushort Height;
            public ushort Depth;
            public ushort MipLevels;
            public fixed uint LodOffset[3];
            public fixed uint OffsetToSurface[13];
        };

        public bool ValidFormat { get; private set; } = false;
        public byte[] ImageData { get; private set; } // decompressed into ARGB or whatever. used for image previews
        public TexHeader Header { get; private set; }
        public bool Local { get; private set; } = false; // was this loaded from the game using Lumina, or from a local ATEX?

        private static int HeaderLength => Unsafe.SizeOf<TexHeader>();
        private byte[] RawData; // just the data, without the header. only used for local files

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;
            Header = Reader.ReadStructure<TexHeader>();
            ImageData = BgraToRgba( Convert( DataSpan[HeaderLength..], Header.Format, Header.Width, Header.Height ) );
            ValidFormat = ( ImageData.Length > 0 );
        }

        public void LoadFile( BinaryReader br, int size ) {
            Local = true;
            br.BaseStream.Position = 0;
            Header = br.ReadStructure<TexHeader>();
            RawData = br.ReadBytes( size - HeaderLength );
            ImageData = BgraToRgba( Convert( new Span<byte>( RawData ), Header.Format, Header.Width, Header.Height ) );
            ValidFormat = ( ImageData.Length > 0 );
        }

        public static TextureFile LoadFromLocal( string path ) {
            var tex = new TextureFile();
            var file = File.Open( path, FileMode.Open );
            using( var reader = new BinaryReader( file ) ) {
                tex.LoadFile( reader, ( int )file.Length );
            }
            file.Close();
            return tex;
        }

        public byte[] GetDdsData() => Local ? RawData : DataSpan[HeaderLength..].ToArray();

        public void SaveAsPng( string path ) {
            var data = new RGBAQuad[Header.Height * Header.Width];
            for( var i = 0; i < Header.Height; i++ ) {
                for( var j = 0; j < Header.Width; j++ ) {
                    var dataIdx = ( i * Header.Width + j );
                    var imageDataIdx = dataIdx * 4;
                    data[dataIdx] = new RGBAQuad( ImageData[imageDataIdx], ImageData[imageDataIdx + 1], ImageData[imageDataIdx + 2], ImageData[imageDataIdx + 3] );
                }
            }
            var ptr = MemoryHelper.PinObject( data );
            var image = Surface.LoadFromRawData( ptr, Header.Width, Header.Height, Header.Width * 4, false, true );
            if( image == null ) return;

            image.SaveToFile( ImageFormat.PNG, path );
        }

        public void SaveAsDds( string path ) {
            var header = AtexUtils.CreateDdsHeader( Header.Width, Header.Height, Header.Format, Header.Depth, Header.MipLevels );
            var data = GetDdsData();
            var writeData = new byte[header.Length + data.Length];
            Buffer.BlockCopy( header, 0, writeData, 0, header.Length );
            Buffer.BlockCopy( data, 0, writeData, header.Length, data.Length );
            File.WriteAllBytes( path, writeData );
        }

        // converts various formats to A8R8G8B8
        public static byte[] Convert( Span<byte> src, TextureFormat format, int width, int height ) {
            var dst = new byte[width * height * 4];
            switch( format ) {
                case TextureFormat.DXT1:
                    DecompressDxt1( src, dst, width, height );
                    break;
                case TextureFormat.DXT3:
                    DecompressDxt3( src, dst, width, height );
                    break;
                case TextureFormat.DXT5:
                    DecompressDxt5( src, dst, width, height );
                    break;
                case TextureFormat.A8R8G8B8:
                    dst = new byte[src.Length];
                    src.CopyTo( dst );
                    break;
                case TextureFormat.A8:
                    DecompressA8( src, dst, width, height );
                    break;
                default:
                    return Array.Empty<byte>(); // ???
            }
            return dst;
        }

        public static TextureFormat DXGItoTextureFormat( DXGIFormat format ) {
            return format switch {
                DXGIFormat.A8_UNorm => TextureFormat.A8,
                DXGIFormat.BC1_UNorm => TextureFormat.DXT1,
                DXGIFormat.BC2_UNorm => TextureFormat.DXT3,
                DXGIFormat.BC3_UNorm => TextureFormat.DXT5,
                DXGIFormat.B8G8R8A8_UNorm => TextureFormat.A8R8G8B8,
                _ => TextureFormat.Null,
            };
        }

        public static CompressionFormat TextureToCompressionFormat( TextureFormat format ) {
            return format switch {
                TextureFormat.DXT1 => CompressionFormat.BC1a,
                TextureFormat.DXT3 => CompressionFormat.BC2,
                TextureFormat.DXT5 => CompressionFormat.BC3,
                TextureFormat.A8R8G8B8 or TextureFormat.A8 => CompressionFormat.BGRA,
                _ => CompressionFormat.ETC1,
            };
        }

        public static byte[] CompressA8( byte[] data ) {
            var ret = new byte[data.Length / 4];
            for( var i = 0; i < ret.Length; i++ ) {
                ret[i] = data[i * 4 + 3];
            }
            return ret;
        }

        public static void DecompressA8( Span<byte> src, byte[] dst, int width, int height ) {
            for( var i = 0; i < width * height; i += 1 ) {
                var idx = i * 4;
                dst[idx + 0] = 0xFF;
                dst[idx + 1] = 0xFF;
                dst[idx + 2] = 0xFF;
                dst[idx + 3] = src[i];
            }
        }

        public static void DecompressDxt1( Span<byte> src, byte[] dst, int width, int height ) {
            var dec = Squish.DecompressImage( src.ToArray(), width, height, SquishOptions.DXT1 );
            Array.Copy( dec, dst, dst.Length );
        }

        public static void DecompressDxt3( Span<byte> src, byte[] dst, int width, int height ) {
            var dec = Squish.DecompressImage( src.ToArray(), width, height, SquishOptions.DXT3 );
            Array.Copy( dec, dst, dst.Length );
        }

        public static void DecompressDxt5( Span<byte> src, byte[] dst, int width, int height ) {
            var dec = Squish.DecompressImage( src.ToArray(), width, height, SquishOptions.DXT5 );
            Array.Copy( dec, dst, dst.Length );
        }

        public static byte[] BgraToRgba( byte[] data ) {
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
            TextureTypeCube = 0x2000000,
            TextureTypeMask = 0x3C00000,
            TextureSwizzle = 0x4000000,
            TextureNoTiled = 0x8000000,
            TextureNoSwizzle = 0x80000000,
        }
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
    }
}
