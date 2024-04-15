using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using VfxEditor.Formats.TextureFormat;

namespace VfxEditor.Utils {
    public static class TextureUtils {
        public static byte[] CreateTextureHeader( TextureFormat format, int width, int height, int mipLevels ) {
            var header = new TextureDataFile.TexHeader() {
                Type = Formats.TextureFormat.Attribute.TextureType2D,
                Format = format,
                Width = ( ushort )width,
                Height = ( ushort )height,
                Depth = 1,
                MipLevels = ( ushort )mipLevels,
                LodOffset = [0, 1, 2],
                OffsetToSurface = new uint[13]
            };
            Array.Clear( header.OffsetToSurface, 0, 13 );

            var mipSize = format switch {
                TextureFormat.R4G4B4A4 or TextureFormat.R5G5B5A1 => ( width * height ) * 2,
                TextureFormat.DXT1 => ( width * height ) / 2,
                TextureFormat.DXT5 or TextureFormat.A8 => width * height,
                _ => ( width * height ) * 4,
            };

            var combinedLength = 80;
            for( var i = 0; i < mipLevels; i++ ) {
                header.OffsetToSurface[i] = ( uint )combinedLength;
                combinedLength += mipSize;
                if( mipSize > 16 ) mipSize /= 4;
                else mipSize = 16;
            }

            // Struct to bytes
            var size = Marshal.SizeOf( header );
            var buffer = new byte[size];
            var handle = Marshal.AllocHGlobal( size );
            Marshal.StructureToPtr( header, handle, true );
            Marshal.Copy( handle, buffer, 0, size );
            Marshal.FreeHGlobal( handle );

            return buffer;
        }

        // https://github.com/TexTools/xivModdingFramework/blob/902ca589fa7548ce4517f886c9775d1c9c5d965e/xivModdingFramework/Textures/FileTypes/DDS.cs

        public static byte[] CreateDdsHeader( uint width, uint height, TextureFormat format, uint depth, uint mipLevels ) {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            writer.Write( 0x20534444u ); // Magic
            writer.Write( 124u ); // Header size
            writer.Write( depth > 1 ? 0x00000004u : 528391u ); // flags
            writer.Write( height );
            writer.Write( width );

            var mipSize = format switch {
                TextureFormat.A8R8G8B8 => ( height * width ) * 4,
                TextureFormat.DXT1 => ( height * width ) / 2,
                TextureFormat.R4G4B4A4 or TextureFormat.R5G5B5A1 => ( height * width ) * 2,
                _ => height * width
            };

            writer.Write( mipSize );
            writer.Write( 0u );
            writer.Write( mipLevels );
            writer.Write( new byte[44] );
            writer.Write( 32u );

            var pfFlags = format switch {
                TextureFormat.A8R8G8B8 or TextureFormat.R4G4B4A4 or TextureFormat.R5G5B5A1 => 65,
                TextureFormat.A8 => 2,
                _ => 4,
            };
            writer.Write( pfFlags );

            uint fourccDX10 = 0x30315844; // "DX10"

            uint magic;
            switch( format ) {
                case TextureFormat.DXT1:
                    magic = 0x31545844;
                    break;
                case TextureFormat.DXT3:
                    magic = 0x33545844;
                    break;
                case TextureFormat.DXT5:
                    magic = 0x35545844;
                    break;
                case TextureFormat.BC5:
                case TextureFormat.BC7:
                    magic = fourccDX10;
                    break;
                case TextureFormat.A8R8G8B8:
                case TextureFormat.R4G4B4A4:
                case TextureFormat.R5G5B5A1:
                case TextureFormat.A8:
                    magic = 0;
                    break;
                default:
                    return null;
            }

            if( depth > 1 ) magic = fourccDX10;

            writer.Write( magic );

            switch( format ) {
                case TextureFormat.A8R8G8B8: {
                        writer.Write( 32u ); // Number of rbg (+a) bits
                        writer.Write( 16711680u ); // Red mask
                        writer.Write( 65280u ); // Green mask
                        writer.Write( 255u ); // Blue mask
                        writer.Write( 4278190080u ); // Alpha mask
                        writer.Write( 4096u ); // Complexity
                        writer.Write( new byte[16] );
                        break;
                    }
                case TextureFormat.R4G4B4A4: {
                        writer.Write( 16u );
                        writer.Write( 3840u );
                        writer.Write( 240u );
                        writer.Write( 15u );
                        writer.Write( 61440u );
                        writer.Write( 4096u );
                        writer.Write( new byte[16] );
                        break;
                    }
                case TextureFormat.R5G5B5A1: {
                        writer.Write( 16u );
                        writer.Write( 31744u );
                        writer.Write( 992u );
                        writer.Write( 31u );
                        writer.Write( 32768u );
                        writer.Write( 4096u );
                        writer.Write( new byte[16] );
                        break;
                    }
                case TextureFormat.A8: {
                        writer.Write( 8u );
                        writer.Write( 0u );
                        writer.Write( 0u );
                        writer.Write( 0u );
                        writer.Write( 255u );
                        writer.Write( 4096u );
                        writer.Write( new byte[16] );
                        break;
                    }
                default: {
                        writer.Write( new byte[40] );
                        break;
                    }
            }

            if( magic == fourccDX10 ) {
                var dxgiFormat = format switch {
                    TextureFormat.DXT1 => DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM,
                    TextureFormat.DXT5 => DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM,
                    TextureFormat.BC5 => DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM,
                    TextureFormat.BC7 => DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM,
                    _ => DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM
                };
                writer.Write( ( uint )dxgiFormat );

                // D3D10_RESOURCE_DIMENSION resourceDimension
                writer.Write( 3 );
                // UINT miscFlag
                writer.Write( 0 );
                // UINT arraySize
                writer.Write( depth );
                // UINT miscFlags2
                writer.Write( 0 );
            }

            return ms.ToArray();
        }

        public static int GetMipSize( TextureFormat format, int width, int height ) => format switch {
            TextureFormat.DXT1 => ( width * height ) / 2,
            TextureFormat.DXT5 or TextureFormat.A8 => width * height,
            TextureFormat.R5G5B5A1 or TextureFormat.R4G4B4A4 => width * height * 2,
            _ => width * height * 4
        };

        public static void GetDdsInfo( byte[] data, TextureFormat format, int width, int height, int mipCount, out byte[] compressedOut, out List<short> mipPartOffsets, out List<short> mipPartCounts ) {
            using var dataMs = new MemoryStream( data );
            using var reader = new BinaryReader( dataMs );
            using var compressedMs = new MemoryStream();
            using var writer = new BinaryWriter( compressedMs );

            mipPartOffsets = [];
            mipPartCounts = [];

            var mipLength = GetMipSize( format, width, height );

            // already skipped the DDS header

            for( var i = 0; i < mipCount; i++ ) {
                var mipParts = ( int )Math.Ceiling( mipLength / 16000f );
                mipPartCounts.Add( ( short )mipParts );

                if( mipParts > 1 ) {
                    for( var j = 0; j < mipParts; j++ ) {
                        GetMipPartOffset( writer, reader, mipPartOffsets, mipLength, j == mipParts - 1 );
                    }
                }
                else {
                    GetMipPartOffset( writer, reader, mipPartOffsets, mipLength, mipLength != 16000 );
                }

                if( mipLength > 32 ) {
                    mipLength /= 4;
                }
                else {
                    mipLength = 8;
                }
            }

            compressedOut = compressedMs.ToArray();
        }

        private static void GetMipPartOffset( BinaryWriter writer, BinaryReader reader, List<short> mipPartOffsets, int mipLength, bool i ) {
            var uncompLength = i ? mipLength % 16000 : 16000;
            var uncompBytes = reader.ReadBytes( uncompLength );
            var compressed = TexToolsUtils.Compressor( uncompBytes );

            var comp = compressed.Length <= uncompLength;
            compressed = comp ? compressed : uncompBytes;

            writer.Write( 16 );
            writer.Write( 0 );
            writer.Write( !comp ? 32000 : compressed.Length );
            writer.Write( uncompLength );
            writer.Write( compressed );

            var padding = 128 - ( compressed.Length % 128 );

            writer.Write( new byte[padding] );

            mipPartOffsets.Add( ( short )( compressed.Length + padding + 16 ) );
        }

        public static byte[] CreateType4Data( TextureFormat format, List<short> mipPartOffsets, List<short> mipPartCount, int uncompressedLength, int mipCount, int width, int height ) {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            var headerSize = 24 + ( mipCount * 20 ) + ( mipPartOffsets.Count * 2 );
            var headerPadding = 128 - ( headerSize % 128 );

            writer.Write( headerSize + headerPadding );
            writer.Write( 4 );
            writer.Write( uncompressedLength + 80 );
            writer.Write( 0 );
            writer.Write( 0 );
            writer.Write( mipCount );

            var uncompMipSize = GetMipSize( format, width, height );
            var partIndex = 0;
            var mipOffsetIndex = 80;

            for( var i = 0; i < mipCount; i++ ) {
                writer.Write( mipOffsetIndex );

                var paddedSize = 0;
                for( var j = 0; j < mipPartCount[i]; j++ ) {
                    paddedSize += mipPartOffsets[j + partIndex];
                }

                writer.Write( paddedSize );
                writer.Write( uncompMipSize > 16 ? uncompMipSize : 16 );

                uncompMipSize /= 4;

                writer.Write( partIndex );
                writer.Write( ( int )mipPartCount[i] );

                partIndex += mipPartCount[i];
                mipOffsetIndex += paddedSize;
            }

            foreach( var part in mipPartOffsets ) writer.Write( part );
            writer.Write( new byte[headerPadding] );

            return ms.ToArray();
        }

        public enum DXGI_FORMAT : uint {
            DXGI_FORMAT_UNKNOWN,
            DXGI_FORMAT_R32G32B32A32_TYPELESS,
            DXGI_FORMAT_R32G32B32A32_FLOAT,
            DXGI_FORMAT_R32G32B32A32_UINT,
            DXGI_FORMAT_R32G32B32A32_SINT,
            DXGI_FORMAT_R32G32B32_TYPELESS,
            DXGI_FORMAT_R32G32B32_FLOAT,
            DXGI_FORMAT_R32G32B32_UINT,
            DXGI_FORMAT_R32G32B32_SINT,
            DXGI_FORMAT_R16G16B16A16_TYPELESS,
            DXGI_FORMAT_R16G16B16A16_FLOAT,
            DXGI_FORMAT_R16G16B16A16_UNORM,
            DXGI_FORMAT_R16G16B16A16_UINT,
            DXGI_FORMAT_R16G16B16A16_SNORM,
            DXGI_FORMAT_R16G16B16A16_SINT,
            DXGI_FORMAT_R32G32_TYPELESS,
            DXGI_FORMAT_R32G32_FLOAT,
            DXGI_FORMAT_R32G32_UINT,
            DXGI_FORMAT_R32G32_SINT,
            DXGI_FORMAT_R32G8X24_TYPELESS,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT,
            DXGI_FORMAT_R10G10B10A2_TYPELESS,
            DXGI_FORMAT_R10G10B10A2_UNORM,
            DXGI_FORMAT_R10G10B10A2_UINT,
            DXGI_FORMAT_R11G11B10_FLOAT,
            DXGI_FORMAT_R8G8B8A8_TYPELESS,
            DXGI_FORMAT_R8G8B8A8_UNORM,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB,
            DXGI_FORMAT_R8G8B8A8_UINT,
            DXGI_FORMAT_R8G8B8A8_SNORM,
            DXGI_FORMAT_R8G8B8A8_SINT,
            DXGI_FORMAT_R16G16_TYPELESS,
            DXGI_FORMAT_R16G16_FLOAT,
            DXGI_FORMAT_R16G16_UNORM,
            DXGI_FORMAT_R16G16_UINT,
            DXGI_FORMAT_R16G16_SNORM,
            DXGI_FORMAT_R16G16_SINT,
            DXGI_FORMAT_R32_TYPELESS,
            DXGI_FORMAT_D32_FLOAT,
            DXGI_FORMAT_R32_FLOAT,
            DXGI_FORMAT_R32_UINT,
            DXGI_FORMAT_R32_SINT,
            DXGI_FORMAT_R24G8_TYPELESS,
            DXGI_FORMAT_D24_UNORM_S8_UINT,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT,
            DXGI_FORMAT_R8G8_TYPELESS,
            DXGI_FORMAT_R8G8_UNORM,
            DXGI_FORMAT_R8G8_UINT,
            DXGI_FORMAT_R8G8_SNORM,
            DXGI_FORMAT_R8G8_SINT,
            DXGI_FORMAT_R16_TYPELESS,
            DXGI_FORMAT_R16_FLOAT,
            DXGI_FORMAT_D16_UNORM,
            DXGI_FORMAT_R16_UNORM,
            DXGI_FORMAT_R16_UINT,
            DXGI_FORMAT_R16_SNORM,
            DXGI_FORMAT_R16_SINT,
            DXGI_FORMAT_R8_TYPELESS,
            DXGI_FORMAT_R8_UNORM,
            DXGI_FORMAT_R8_UINT,
            DXGI_FORMAT_R8_SNORM,
            DXGI_FORMAT_R8_SINT,
            DXGI_FORMAT_A8_UNORM,
            DXGI_FORMAT_R1_UNORM,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP,
            DXGI_FORMAT_R8G8_B8G8_UNORM,
            DXGI_FORMAT_G8R8_G8B8_UNORM,
            DXGI_FORMAT_BC1_TYPELESS,
            DXGI_FORMAT_BC1_UNORM,
            DXGI_FORMAT_BC1_UNORM_SRGB,
            DXGI_FORMAT_BC2_TYPELESS,
            DXGI_FORMAT_BC2_UNORM,
            DXGI_FORMAT_BC2_UNORM_SRGB,
            DXGI_FORMAT_BC3_TYPELESS,
            DXGI_FORMAT_BC3_UNORM,
            DXGI_FORMAT_BC3_UNORM_SRGB,
            DXGI_FORMAT_BC4_TYPELESS,
            DXGI_FORMAT_BC4_UNORM,
            DXGI_FORMAT_BC4_SNORM,
            DXGI_FORMAT_BC5_TYPELESS,
            DXGI_FORMAT_BC5_UNORM,
            DXGI_FORMAT_BC5_SNORM,
            DXGI_FORMAT_B5G6R5_UNORM,
            DXGI_FORMAT_B5G5R5A1_UNORM,
            DXGI_FORMAT_B8G8R8A8_UNORM,
            DXGI_FORMAT_B8G8R8X8_UNORM,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM,
            DXGI_FORMAT_B8G8R8A8_TYPELESS,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,
            DXGI_FORMAT_B8G8R8X8_TYPELESS,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB,
            DXGI_FORMAT_BC6H_TYPELESS,
            DXGI_FORMAT_BC6H_UF16,
            DXGI_FORMAT_BC6H_SF16,
            DXGI_FORMAT_BC7_TYPELESS,
            DXGI_FORMAT_BC7_UNORM,
            DXGI_FORMAT_BC7_UNORM_SRGB,
            DXGI_FORMAT_AYUV,
            DXGI_FORMAT_Y410,
            DXGI_FORMAT_Y416,
            DXGI_FORMAT_NV12,
            DXGI_FORMAT_P010,
            DXGI_FORMAT_P016,
            DXGI_FORMAT_420_OPAQUE,
            DXGI_FORMAT_YUY2,
            DXGI_FORMAT_Y210,
            DXGI_FORMAT_Y216,
            DXGI_FORMAT_NV11,
            DXGI_FORMAT_AI44,
            DXGI_FORMAT_IA44,
            DXGI_FORMAT_P8,
            DXGI_FORMAT_A8P8,
            DXGI_FORMAT_B4G4R4A4_UNORM,
            DXGI_FORMAT_P208,
            DXGI_FORMAT_V208,
            DXGI_FORMAT_V408,
            DXGI_FORMAT_SAMPLER_FEEDBACK_MIN_MIP_OPAQUE,
            DXGI_FORMAT_SAMPLER_FEEDBACK_MIP_REGION_USED_OPAQUE,
            DXGI_FORMAT_FORCE_UINT
        };
    }
}
