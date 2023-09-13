using System;
using System.Collections.Generic;
using System.Text;
using VfxEditor.Formats.TextureFormat;

namespace VfxEditor.Utils {
    // https://github.com/TexTools/xivModdingFramework/blob/902ca589fa7548ce4517f886c9775d1c9c5d965e/xivModdingFramework/Textures/FileTypes/DDS.cs
    public static class TextureUtils {
        public static List<byte> CreateTextureHeader( TextureFormat format, int newWidth, int newHeight, int newMipCount ) {
            var headerData = new List<byte>();

            short texFormatCode = 0;
            switch( format ) {
                case TextureFormat.DXT1:
                    texFormatCode = 13344;
                    break;
                case TextureFormat.DXT3:
                    texFormatCode = 13360;
                    break;
                case TextureFormat.DXT5:
                    texFormatCode = 13361;
                    break;
                case TextureFormat.A8:
                    texFormatCode = 4401;
                    break;
                case TextureFormat.A8R8G8B8:
                    texFormatCode = 5200;
                    break;
                case TextureFormat.R4G4B4A4:
                    texFormatCode = 5184;
                    break;
            }

            headerData.AddRange( BitConverter.GetBytes( ( short )0 ) );
            headerData.AddRange( BitConverter.GetBytes( ( short )128 ) );
            headerData.AddRange( BitConverter.GetBytes( texFormatCode ) );
            headerData.AddRange( BitConverter.GetBytes( ( short )0 ) );
            headerData.AddRange( BitConverter.GetBytes( ( short )newWidth ) );
            headerData.AddRange( BitConverter.GetBytes( ( short )newHeight ) );
            headerData.AddRange( BitConverter.GetBytes( ( short )1 ) );
            headerData.AddRange( BitConverter.GetBytes( ( short )newMipCount ) );
            headerData.AddRange( BitConverter.GetBytes( 0 ) );
            headerData.AddRange( BitConverter.GetBytes( 1 ) );
            headerData.AddRange( BitConverter.GetBytes( 2 ) );
            var mipLength = format switch {
                TextureFormat.R4G4B4A4 => ( newWidth * newHeight ) * 2,
                TextureFormat.DXT1 => ( newWidth * newHeight ) / 2,
                TextureFormat.DXT5 or TextureFormat.A8 => newWidth * newHeight,
                _ => ( newWidth * newHeight ) * 4,
            };

            var combinedLength = 80;
            for( var i = 0; i < newMipCount; i++ ) {
                headerData.AddRange( BitConverter.GetBytes( combinedLength ) );
                combinedLength += mipLength;
                if( mipLength > 16 ) {
                    mipLength /= 4;
                }
                else {
                    mipLength = 16;
                }
            }

            var padding = 80 - headerData.Count;
            headerData.AddRange( new byte[padding] );
            return headerData;
        }

        public static byte[] CreateDdsHeader( ushort width, ushort height, TextureFormat format, ushort depth, ushort mipLevels ) {
            uint dwPitchOrLinearSize, dwFourCC;
            var header = new List<byte>();

            header.AddRange( BitConverter.GetBytes( ( uint )0x20534444 ) );
            header.AddRange( BitConverter.GetBytes( ( uint )124 ) );

            uint dwFlags = 528391;
            if( depth > 1 ) {
                dwFlags = 0x00000004;
            }
            header.AddRange( BitConverter.GetBytes( dwFlags ) );

            var dwHeight = ( uint )height;
            header.AddRange( BitConverter.GetBytes( dwHeight ) );
            var dwWidth = ( uint )width;
            header.AddRange( BitConverter.GetBytes( dwWidth ) );

            if( format == TextureFormat.A8R8G8B8 ) {
                dwPitchOrLinearSize = ( dwHeight * dwWidth ) * 4;
            }
            else if( format == TextureFormat.DXT1 ) {
                dwPitchOrLinearSize = ( dwHeight * dwWidth ) / 2;
            }
            else if( format == TextureFormat.R4G4B4A4 ) {
                dwPitchOrLinearSize = ( dwHeight * dwWidth ) * 2;
            }
            else {
                dwPitchOrLinearSize = dwHeight * dwWidth;
            }
            header.AddRange( BitConverter.GetBytes( dwPitchOrLinearSize ) );

            header.AddRange( BitConverter.GetBytes( ( uint )0 ) );
            header.AddRange( BitConverter.GetBytes( ( uint )mipLevels ) );
            var dwReserved1 = new byte[44];
            Array.Clear( dwReserved1, 0, 44 );
            header.AddRange( dwReserved1 );

            header.AddRange( BitConverter.GetBytes( ( uint )32 ) );
            var pfFlags = format switch {
                TextureFormat.A8R8G8B8 or TextureFormat.R4G4B4A4 => 65,
                TextureFormat.A8 => 2,
                _ => 4,
            };
            header.AddRange( BitConverter.GetBytes( pfFlags ) );

            switch( format ) {
                case TextureFormat.DXT1:
                    dwFourCC = 0x31545844;
                    break;
                case TextureFormat.DXT3:
                    dwFourCC = 0x33545844;
                    break;
                case TextureFormat.DXT5:
                    dwFourCC = 0x35545844;
                    break;
                case TextureFormat.A8R8G8B8:
                case TextureFormat.R4G4B4A4:
                case TextureFormat.A8:
                    dwFourCC = 0;
                    break;
                default:
                    return null;
            }

            if( depth > 1 ) {
                var bytes = Encoding.UTF8.GetBytes( "DX10" );
                dwFourCC = BitConverter.ToUInt32( bytes, 0 );
            }

            header.AddRange( BitConverter.GetBytes( dwFourCC ) );

            switch( format ) {
                case TextureFormat.A8R8G8B8: {
                        const uint dwRGBBitCount = 32;
                        header.AddRange( BitConverter.GetBytes( dwRGBBitCount ) );
                        const uint dwRBitMask = 16711680;
                        header.AddRange( BitConverter.GetBytes( dwRBitMask ) );
                        const uint dwGBitMask = 65280;
                        header.AddRange( BitConverter.GetBytes( dwGBitMask ) );
                        const uint dwBBitMask = 255;
                        header.AddRange( BitConverter.GetBytes( dwBBitMask ) );
                        const uint dwABitMask = 4278190080;
                        header.AddRange( BitConverter.GetBytes( dwABitMask ) );
                        const uint dwCaps = 4096;
                        header.AddRange( BitConverter.GetBytes( dwCaps ) );
                        var blank1 = new byte[16];
                        header.AddRange( blank1 );

                        break;
                    }
                case TextureFormat.R4G4B4A4: {
                        const uint dwRGBBitCount = 16;
                        header.AddRange( BitConverter.GetBytes( dwRGBBitCount ) );
                        const uint dwRBitMask = 3840;
                        header.AddRange( BitConverter.GetBytes( dwRBitMask ) );
                        const uint dwGBitMask = 240;
                        header.AddRange( BitConverter.GetBytes( dwGBitMask ) );
                        const uint dwBBitMask = 15;
                        header.AddRange( BitConverter.GetBytes( dwBBitMask ) );
                        const uint dwABitMask = 61440;
                        header.AddRange( BitConverter.GetBytes( dwABitMask ) );
                        const uint dwCaps = 4096;
                        header.AddRange( BitConverter.GetBytes( dwCaps ) );
                        var blank1 = new byte[16];
                        header.AddRange( blank1 );
                        break;
                    }
                case TextureFormat.A8: {
                        const uint dwRGBBitCount = 8;
                        header.AddRange( BitConverter.GetBytes( dwRGBBitCount ) );
                        const uint dwRBitMask = 0;
                        header.AddRange( BitConverter.GetBytes( dwRBitMask ) );
                        const uint dwGBitMask = 0;
                        header.AddRange( BitConverter.GetBytes( dwGBitMask ) );
                        const uint dwBBitMask = 0;
                        header.AddRange( BitConverter.GetBytes( dwBBitMask ) );
                        const uint dwABitMask = 255;
                        header.AddRange( BitConverter.GetBytes( dwABitMask ) );
                        const uint dwCaps = 4096;
                        header.AddRange( BitConverter.GetBytes( dwCaps ) );
                        var blank1 = new byte[16];
                        header.AddRange( blank1 );
                        break;
                    }
                default: {
                        var blank1 = new byte[40];
                        header.AddRange( blank1 );
                        break;
                    }
            }

            if( depth > 1 ) {
                uint dxgiFormat;
                if( format == TextureFormat.DXT1 ) {
                    dxgiFormat = ( uint )DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
                }
                else if( format == TextureFormat.DXT5 ) {
                    dxgiFormat = ( uint )DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
                }
                else {
                    dxgiFormat = ( uint )DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                }
                header.AddRange( BitConverter.GetBytes( dxgiFormat ) );

                // D3D10_RESOURCE_DIMENSION resourceDimension
                header.AddRange( BitConverter.GetBytes( 3 ) );
                // UINT miscFlag
                header.AddRange( BitConverter.GetBytes( 0 ) );
                // UINT arraySize
                header.AddRange( BitConverter.GetBytes( depth ) );
                // UINT miscFlags2
                header.AddRange( BitConverter.GetBytes( 0 ) );
            }

            return header.ToArray();
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
