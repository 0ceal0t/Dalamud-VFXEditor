using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using Lumina.Data.Parsing.Tex;
using Lumina.Extensions;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;

namespace VFXEditor.Data.Texture {  

    public class VFXTexture : Lumina.Data.FileResource
    {
        [StructLayout( LayoutKind.Sequential )]
        public unsafe struct TexHeader
        {
            public Attribute Type;
            public TextureFormat Format;
            public ushort Width;
            public ushort Height;
            public ushort Depth;
            public ushort MipLevels;
            public fixed uint LodOffset[3];
            public fixed uint OffsetToSurface[13];
        };
        public TexHeader Header;
        public int HeaderLength => Unsafe.SizeOf<TexHeader>();

        public bool ValidFormat = false;
        public bool Local = false; // was this loaded from the game using Lumina, or from a local ATEX?

        public byte[] RawData; // just the data, without the header. only used for local files
        public byte[] ImageData { get; private set; } // decompressed into ARGB or whatever. used for image previews

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;
            Header = Reader.ReadStructure<TexHeader>();
            ImageData = BGRA_to_RGBA( Convert( DataSpan.Slice( HeaderLength ), Header.Width, Header.Height ) );
        }

        public void LoadFileLocal(BinaryReader br, int size) {
            Local = true;
            br.BaseStream.Position = 0;
            Header = br.ReadStructure<TexHeader>();
            RawData = br.ReadBytes( size - HeaderLength );
            ImageData = BGRA_to_RGBA( Convert( new Span<byte>(RawData), Header.Width, Header.Height ) );
        }
        public byte[] GetDDSData() {
            if( !Local ) {
                return DataSpan.Slice( HeaderLength ).ToArray();
            }
            else {
                return RawData;
            }
        }

        public static VFXTexture LoadFromLocal( string path ) {
            VFXTexture tex = new VFXTexture();
            var file = File.Open( path, FileMode.Open );
            using( BinaryReader reader = new BinaryReader( file ) ) {
                tex.LoadFileLocal( reader, ( int )file.Length );
            }
            file.Close();
            return tex;
        }

        // converts various formats to A8R8G8B8
        private byte[] Convert( Span<byte> src, int width, int height ) {
            byte[] dst = new byte[width * height * 4];
            switch( Header.Format ) {
                case TextureFormat.DXT1:
                    DecompressDxt1( src, dst, width, height );
                    break;
                case TextureFormat.DXT5:
                    DecompressDxt5( src, dst, width, height );
                    break;
                case TextureFormat.A8R8G8B8:
                    if(dst.Length != src.Length ) { // ???
                        return new byte[0];
                    }
                    src.CopyTo( dst );
                    break;
                case TextureFormat.A8:
                    DecompressA8( src, dst, width, height );
                    break;
                default:
                    return new byte[0]; // ???
            }
            ValidFormat = true;
            return dst;
        }

        public static TextureFormat DXGItoTextureFormat(DXGIFormat format ) {
            switch( format ) {
                case DXGIFormat.A8_UNorm:
                    return TextureFormat.A8;
                case DXGIFormat.BC1_UNorm:
                    return TextureFormat.DXT1;
                case DXGIFormat.BC3_UNorm:
                    return TextureFormat.DXT5;
                case DXGIFormat.B8G8R8A8_UNorm:
                    return TextureFormat.A8R8G8B8;
                default:
                    return TextureFormat.Null;
            }
        }
        public static CompressionFormat TextureToCompressionFormat( TextureFormat format ) {
            switch( format ) {
                case TextureFormat.DXT1:
                    return CompressionFormat.BC1a;
                case TextureFormat.DXT5:
                    return CompressionFormat.BC3;
                case TextureFormat.A8R8G8B8:
                case TextureFormat.A8:
                    return CompressionFormat.BGRA;
                default:
                    return CompressionFormat.ETC1;
            }
        }

        public static byte[] CompressA8(byte[] data) {
            byte[] ret = new byte[data.Length / 4];
            for(int i = 0; i < ret.Length; i++ ) {
                ret[i] = data[i * 4 + 3];
            }
            return ret;
        }
        private static void DecompressA8( Span<byte> src, byte[] dst, int width, int height ) {
            for( var i = 0; i < width * height; i += 1 ) {
                int idx = i * 4;
                dst[idx + 0] = 0xFF;
                dst[idx + 1] = 0xFF;
                dst[idx + 2] = 0xFF;
                dst[idx + 3] = src[i];
            }
        }
        private static void DecompressDxt1( Span<byte> src, byte[] dst, int width, int height ) {
            var dec = Squish.DecompressImage( src.ToArray(), width, height, SquishOptions.DXT1 );
            Array.Copy( dec, dst, dst.Length );
        }
        private static void DecompressDxt5( Span<byte> src, byte[] dst, int width, int height ) {
            var dec = Squish.DecompressImage( src.ToArray(), width, height, SquishOptions.DXT5 );
            Array.Copy( dec, dst, dst.Length );
        }

        public static byte[] BGRA_to_RGBA( byte[] data ) {
            byte[] ret = new byte[data.Length];
            for( int i = 0; i < data.Length / 4; i++ ) {
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
        Unknown = 0x0,
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
        TypeDepthStencil = 0x4,
        TypeSpecial = 0x5,
        A8R8G8B8 = 0x1450,
        // todo:
        R8G8B8X8 = 0x1451,
        A8R8G8B82 = 0x1452,
        R4G4B4A4 = 0x1440,
        R5G5B5A1 = 0x1441,
        L8 = 0x1130,
        // todo:
        A8 = 0x1131,
        // todo:
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
