using Dalamud.Logging;
using SharpDX.D3DCompiler;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Texture {
    public class TstDDSFile {
        /*public static bool Write( Stream output, List<TeximpNet.DDS.MipChain> mipChains, TeximpNet.DDS.DXGIFormat format, TeximpNet.DDS.TextureDimension texDim, TeximpNet.DDS.DDSFlags flags = TeximpNet.DDS.DDSFlags.None ) {
            if( output == null || !output.CanWrite || mipChains == null || mipChains.Count == 0 || mipChains[0].Count == 0 || format == DXGIFormat.Unknown )
                return false;

            //Extract details
            int width, height, depth, arrayCount, mipCount;
            TeximpNet.DDS.MipData firstMip = mipChains[0][0];
            width = firstMip.Width;
            height = firstMip.Height;
            depth = firstMip.Depth;
            arrayCount = mipChains.Count;
            mipCount = mipChains[0].Count;

            if( !ValidateInternal( mipChains, format, texDim ) )
                return false;

            //Setup a transfer buffer
            StreamTransferBuffer buffer = new StreamTransferBuffer( firstMip.RowPitch, false );

            //Write out header
            if( !WriteHeader( output, buffer, texDim, format, width, height, depth, arrayCount, mipCount, flags ) )
                return false;

            //Iterate over each array face...
            for( int i = 0; i < arrayCount; i++ ) {
                MipChain mipChain = mipChains[i];

                //Iterate over each mip face...
                for( int mipLevel = 0; mipLevel < mipCount; mipLevel++ ) {
                    MipData mip = mipChain[mipLevel];

                    //Compute pitch, based on MSDN programming guide. We will write out these pitches rather than the supplied in order to conform to the recomendation
                    //that we compute pitch based on format
                    int realMipWidth, realMipHeight, dstRowPitch, dstSlicePitch, bytesPerPixel;
                    ImageHelper.ComputePitch( format, mip.Width, mip.Height, out dstRowPitch, out dstSlicePitch, out realMipWidth, out realMipHeight, out bytesPerPixel );

                    //Ensure write buffer is sufficiently sized for a single scanline
                    if( buffer.Length < dstRowPitch )
                        buffer.Resize( dstRowPitch, false );

                    //Sanity check
                    if( dstRowPitch < mip.RowPitch )
                        return false;

                    IntPtr srcPtr = mip.Data;

                    //Advance stream one slice at a time...
                    for( int slice = 0; slice < mip.Depth; slice++ ) {
                        int bytesToWrite = dstSlicePitch;
                        IntPtr sPtr = srcPtr;

                        //Copy scanline into temp buffer, write to output
                        for( int row = 0; row < realMipHeight; row++ ) {
                            MemoryHelper.CopyMemory( buffer.Pointer, sPtr, dstRowPitch );
                            buffer.WriteBytes( output, dstRowPitch );
                            bytesToWrite -= dstRowPitch;

                            //Advance to next scanline in source data
                            sPtr = MemoryHelper.AddIntPtr( sPtr, mip.RowPitch );
                        }

                        //Pad slice if necessary
                        if( bytesToWrite > 0 ) {
                            MemoryHelper.ClearMemory( buffer.Pointer, 0, bytesToWrite );
                            buffer.WriteBytes( output, bytesToWrite );
                        }

                        //Advance source pointer to next slice
                        srcPtr = MemoryHelper.AddIntPtr( srcPtr, mip.SlicePitch );
                    }
                }
            }

            return true;
        }*/

        public static bool WriteHeader( Stream output, TeximpNet.StreamTransferBuffer buffer, TeximpNet.DDS.TextureDimension texDim, TeximpNet.DDS.DXGIFormat format, int width, int height, int depth, int arrayCount, int mipCount, TeximpNet.DDS.DDSFlags flags ) {
            //Force the DX10 header...
            bool writeDX10Header = ( flags & TeximpNet.DDS.DDSFlags.ForceExtendedHeader ) == TeximpNet.DDS.DDSFlags.ForceExtendedHeader;

            //Or do DX10 if the following is true...1D textures or 2D texture arrays that aren't cubemaps...
            if( !writeDX10Header ) {
                switch( texDim ) {
                    case TeximpNet.DDS.TextureDimension.One:
                        writeDX10Header = true;
                        break;
                    case TeximpNet.DDS.TextureDimension.Two:
                        writeDX10Header = arrayCount > 1;
                        break;
                }
            }

            PluginLog.Log( $"{writeDX10Header}" );

            //Figure out pixel format, if not writing DX10 header...
            PixelFormat pixelFormat;
            if( !writeDX10Header ) {
                switch( format ) {
                    case TeximpNet.DDS.DXGIFormat.R8G8B8A8_UNorm:
                        pixelFormat = PixelFormat.A8B8G8R8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16G16_UNorm:
                        pixelFormat = PixelFormat.G16R16;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R8G8_UNorm:
                        pixelFormat = PixelFormat.A8L8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16_UNorm:
                        pixelFormat = PixelFormat.L16;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R8_UNorm:
                        pixelFormat = PixelFormat.L8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.A8_UNorm:
                        pixelFormat = PixelFormat.A8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R8G8_B8G8_UNorm:
                        pixelFormat = PixelFormat.R8G8_B8G8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.G8R8_G8B8_UNorm:
                        pixelFormat = PixelFormat.G8R8_G8B8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC1_UNorm:
                        pixelFormat = PixelFormat.DXT1;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC2_UNorm:
                        pixelFormat = PixelFormat.DXT3;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC3_UNorm:
                        pixelFormat = PixelFormat.DXT5;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC4_UNorm:
                        pixelFormat = PixelFormat.BC4_UNorm;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC4_SNorm:
                        pixelFormat = PixelFormat.BC4_SNorm;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC5_UNorm:
                        pixelFormat = PixelFormat.BC5_UNorm;
                        break;
                    case TeximpNet.DDS.DXGIFormat.BC5_SNorm:
                        pixelFormat = PixelFormat.BC5_SNorm;
                        break;
                    case TeximpNet.DDS.DXGIFormat.B5G6R5_UNorm:
                        pixelFormat = PixelFormat.R5G6B5;
                        break;
                    case TeximpNet.DDS.DXGIFormat.B5G5R5A1_UNorm:
                        pixelFormat = PixelFormat.A1R5G5B5;
                        break;
                    case TeximpNet.DDS.DXGIFormat.B8G8R8A8_UNorm:
                        pixelFormat = PixelFormat.A8R8G8B8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.B8G8R8X8_UNorm:
                        pixelFormat = PixelFormat.X8R8G8B8;
                        break;
                    case TeximpNet.DDS.DXGIFormat.B4G4R4A4_UNorm:
                        pixelFormat = PixelFormat.A4R4G4B4;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R32G32B32A32_Float:
                        pixelFormat = PixelFormat.R32G32B32A32_Float;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16G16B16A16_Float:
                        pixelFormat = PixelFormat.R16G16B16A16_Float;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16G16B16A16_UNorm:
                        pixelFormat = PixelFormat.R16G16B16A16_UNorm;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16G16B16A16_SNorm:
                        pixelFormat = PixelFormat.R16G16B16A16_SNorm;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R32G32_Float:
                        pixelFormat = PixelFormat.R32G32_Float;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16G16_Float:
                        pixelFormat = PixelFormat.R16G16_Float;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R32_Float:
                        pixelFormat = PixelFormat.R32_Float;
                        break;
                    case TeximpNet.DDS.DXGIFormat.R16_Float:
                        pixelFormat = PixelFormat.R16_Float;
                        break;
                    default:
                        pixelFormat = PixelFormat.DX10Extended;
                        writeDX10Header = true;
                        break;
                }
            }
            else {
                pixelFormat = PixelFormat.DX10Extended;
            }

            Header header = new Header();
            header.Size = ( uint )TeximpNet.MemoryHelper.SizeOf<Header>();
            header.PixelFormat = pixelFormat;
            header.Flags = HeaderFlags.Caps | HeaderFlags.Width | HeaderFlags.Height | HeaderFlags.PixelFormat;
            header.Caps = HeaderCaps.Texture;
            Header10? header10 = null;

            if( mipCount > 0 ) {
                header.Flags |= HeaderFlags.MipMapCount;
                header.MipMapCount = ( uint )mipCount;
                header.Caps |= HeaderCaps.MipMap;
            }

            switch( texDim ) {
                case TeximpNet.DDS.TextureDimension.One:
                    header.Width = ( uint )width;
                    header.Height = 1;
                    header.Depth = 1;

                    //Should always be writing out extended header for 1D textures
                    System.Diagnostics.Debug.Assert( writeDX10Header );

                    header10 = new Header10( format, D3D10ResourceDimension.Texture1D, Header10Flags.None, ( uint )arrayCount, Header10Flags2.None );

                    break;
                case TeximpNet.DDS.TextureDimension.Two:
                    header.Width = ( uint )width;
                    header.Height = ( uint )height;
                    header.Depth = 1;

                    if( writeDX10Header )
                        header10 = new Header10( format, D3D10ResourceDimension.Texture2D, Header10Flags.None, ( uint )arrayCount, Header10Flags2.None );

                    break;
                case TeximpNet.DDS.TextureDimension.Cube:
                    header.Width = ( uint )width;
                    header.Height = ( uint )height;
                    header.Depth = 1;
                    header.Caps |= HeaderCaps.Complex;
                    header.Caps2 |= HeaderCaps2.Cubemap_AllFaces;

                    //can support array tex cubes, so must be multiples of 6
                    if( arrayCount % 6 != 0 )
                        return false;

                    if( writeDX10Header )
                        header10 = new Header10( format, D3D10ResourceDimension.Texture2D, Header10Flags.TextureCube, ( uint )arrayCount / 6, Header10Flags2.None );

                    break;
                case TeximpNet.DDS.TextureDimension.Three:
                    header.Width = ( uint )width;
                    header.Height = ( uint )height;
                    header.Depth = ( uint )depth;
                    header.Flags |= HeaderFlags.Depth;
                    header.Caps2 |= HeaderCaps2.Volume;

                    if( arrayCount != 1 )
                        return false;

                    if( writeDX10Header )
                        header10 = new Header10( format, D3D10ResourceDimension.Texture3D, Header10Flags.None, 1, Header10Flags2.None );

                    break;
            }

            int realWidth, realHeight, rowPitch, slicePitch;
            TeximpNet.ImageHelper.ComputePitch( format, width, height, out rowPitch, out slicePitch, out realWidth, out realHeight );

            if( IsCompressed( format ) ) {
                header.Flags |= HeaderFlags.LinearSize;
                header.PitchOrLinearSize = ( uint )slicePitch;
            }
            else {
                header.Flags |= HeaderFlags.Pitch;
                header.PitchOrLinearSize = ( uint )rowPitch;
            }

            //Write out magic word, DDS header, and optionally extended header

            int size = MemoryInterop.SizeOfInline<Header>();
            PluginLog.Log( $"Size: {size}" );

            buffer.Write<FourCC>( output, DDS_MAGIC );
            buffer.Write<Header>( output, header ); // here <---------------

            if( header10.HasValue ) {
                System.Diagnostics.Debug.Assert( header.PixelFormat.IsDX10Extended );
                buffer.Write<Header10>( output, header10.Value );
            }

            return true;
        }

        public static bool IsCompressed( TeximpNet.DDS.DXGIFormat format ) {
            switch( format ) {
                case TeximpNet.DDS.DXGIFormat.BC1_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC1_UNorm:
                case TeximpNet.DDS.DXGIFormat.BC1_UNorm_SRGB:
                case TeximpNet.DDS.DXGIFormat.BC2_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC2_UNorm:
                case TeximpNet.DDS.DXGIFormat.BC2_UNorm_SRGB:
                case TeximpNet.DDS.DXGIFormat.BC3_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC3_UNorm:
                case TeximpNet.DDS.DXGIFormat.BC3_UNorm_SRGB:
                case TeximpNet.DDS.DXGIFormat.BC4_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC4_UNorm:
                case TeximpNet.DDS.DXGIFormat.BC4_SNorm:
                case TeximpNet.DDS.DXGIFormat.BC5_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC5_UNorm:
                case TeximpNet.DDS.DXGIFormat.BC5_SNorm:
                case TeximpNet.DDS.DXGIFormat.BC6H_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC6H_UF16:
                case TeximpNet.DDS.DXGIFormat.BC6H_SF16:
                case TeximpNet.DDS.DXGIFormat.BC7_Typeless:
                case TeximpNet.DDS.DXGIFormat.BC7_UNorm:
                case TeximpNet.DDS.DXGIFormat.BC7_UNorm_SRGB:
                    return true;
                default:
                    return false;
            }
        }

        private static readonly FourCC DDS_MAGIC = new FourCC( 'D', 'D', 'S', ' ' );
    }

    /// <summary>
    /// Header for a DDS file, comes right after the "DDS " magic word. If pixel format is set to use extended header, that comes right after this and then the data.
    /// </summary>
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    [DebuggerDisplay( "Width = {Width}, Height = {Height}, Depth = {Depth}, Flags = {Flags}, MipCount = {MipMapCount}, Caps = {Caps}, Caps2 = {Caps2}, PixelFormat = {PixelFormat}" )]
    internal unsafe struct Header {
        public uint Size;
        public HeaderFlags Flags;
        public uint Height;
        public uint Width;
        public uint PitchOrLinearSize;
        public uint Depth;
        public uint MipMapCount;
        public fixed uint Reserved1[11];
        public PixelFormat PixelFormat;
        public HeaderCaps Caps;
        public HeaderCaps2 Caps2;
        public uint Caps3;
        public uint Caps4;
        public uint Reserved2;
    }

    internal enum D3D10ResourceDimension : uint {
        Unknown = 0,
        Buffer = 1,
        Texture1D = 2,
        Texture2D = 3,
        Texture3D = 4
    }

    [Flags]
    internal enum HeaderFlags : uint {
        None = 0,
        Caps = 0x1,
        Height = 0x2,
        Width = 0x4,
        Pitch = 0x8,
        PixelFormat = 0x1000,
        MipMapCount = 0x20000,
        LinearSize = 0x80000,
        Depth = 0x800000
    }

    [Flags]
    internal enum HeaderCaps : uint {
        None = 0,
        Complex = 0x8,
        Texture = 0x1000,
        MipMap = 0x400000
    }

    [Flags]
    internal enum HeaderCaps2 : uint {
        None = 0,
        Cubemap = 0x200,
        Cubemap_PositiveX = Cubemap | 0x400,
        Cubemap_NegativeX = Cubemap | 0x800,
        Cubemap_PositiveY = Cubemap | 0x1000,
        Cubemap_NegativeY = Cubemap | 0x2000,
        Cubemap_PositiveZ = Cubemap | 0x4000,
        Cubemap_NegativeZ = Cubemap | 0x8000,
        Cubemap_AllFaces = Cubemap_PositiveX | Cubemap_NegativeX | Cubemap_PositiveY | Cubemap_NegativeY | Cubemap_PositiveZ | Cubemap_NegativeZ,
        Volume = 0x200000
    }

    [Flags]
    internal enum Header10Flags : uint {
        None = 0,
        TextureCube = 0x4
    }

    [Flags]
    internal enum Header10Flags2 : uint {
        None = 0,
        AlphaModeStraight = 0x1,
        AlphaModePremultiplied = 0x2,
        AlphaModeOpaque = 0x3,
        AlphaModeCustom = 0x4
    }


    [Flags]
    internal enum PixelFormatFlags : uint {
        None = 0,
        AlphaPixels = 0x1, //Has an alpha channel
        Alpha = 0x2, //ONLY has alpha data, some old files use this
        FourCC = 0x4,
        RGB = 0x40,
        RGBA = RGB | AlphaPixels,
        YUV = 0x200,
        Luminance = 0x20000,
        LuminanceAlpha = Luminance | AlphaPixels,
        Pal8 = 0x00000020,
        Pal8Alpha = Pal8 | AlphaPixels,
        BumpDUDV = 0x00080000
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    [DebuggerDisplay( "Format = {Format}, Dimension = {ResourceDimension}, ArraySize = {ArraySize}, MiscFlags = {MiscFlag}, MiscFlags2 = {MiscFlags2}" )]
    internal struct Header10 {
        public TeximpNet.DDS.DXGIFormat Format;
        public D3D10ResourceDimension ResourceDimension;
        public Header10Flags MiscFlags;
        public uint ArraySize;
        public Header10Flags2 MiscFlags2;

        public Header10( TeximpNet.DDS.DXGIFormat format, D3D10ResourceDimension resourceDim, Header10Flags miscFlags, uint arraySize, Header10Flags2 miscFlags2 ) {
            Format = format;
            ResourceDimension = resourceDim;
            MiscFlags = miscFlags;
            ArraySize = arraySize;
            MiscFlags2 = miscFlags2;
        }
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    internal struct PixelFormat {
        public uint Size;
        public PixelFormatFlags Flags;
        public FourCC FourCC;
        public uint RGBBitCount;
        public uint RedBitMask;
        public uint GreenBitMask;
        public uint BlueBitMask;
        public uint AlphaBitMask;

        /// <summary>
        /// Gets a value indicating whether this format description is extended.
        /// </summary>
        public bool IsDX10Extended {
            get {
                return FourCC == new FourCC( 'D', 'X', '1', '0' );
            }
        }

        public PixelFormat( PixelFormatFlags flags, FourCC formatCode, uint colorBitCount, uint redMask, uint greenMask, uint blueMask, uint alphaMask ) {
            Size = ( uint )TeximpNet.MemoryHelper.SizeOf<PixelFormat>();
            Flags = flags;
            FourCC = formatCode;
            RGBBitCount = colorBitCount;
            RedBitMask = redMask;
            GreenBitMask = greenMask;
            BlueBitMask = blueMask;
            AlphaBitMask = alphaMask;
        }

        public PixelFormat( PixelFormatFlags flags, FourCC formatCode ) {
            Size = ( uint )TeximpNet.MemoryHelper.SizeOf<PixelFormat>();
            Flags = flags;
            FourCC = formatCode;
            RGBBitCount = 0;
            RedBitMask = 0;
            GreenBitMask = 0;
            BlueBitMask = 0;
            AlphaBitMask = 0;
        }

        public override string ToString() {
            if( Flags == PixelFormatFlags.FourCC ) {
                //D3D Format enum is sometimes the first char, if so then it's not a proper fourCC and will just appear as a single character. Lets display the integer value instead in this case.
                String fourCCValue = ( FourCC.First > 0 && FourCC.Second == 0 && FourCC.Third == 0 && FourCC.Fourth == 0 ) ? ( ( int )FourCC.First ).ToString() : FourCC.ToString();

                return String.Format( "Flags = {0}, FourCC = '{1}'", Flags.ToString(), fourCCValue );
            }

            return String.Format( "Flags = {0}, BitsPerPixel = {1}, RedMask = 0x{2}, GreenMask = 0x{3}, BlueMask = 0x{4}, AlphaMask = 0x{5}", Flags.ToString(), RGBBitCount.ToString(),
                RedBitMask.ToString( "X8" ), GreenBitMask.ToString( "X8" ), BlueBitMask.ToString( "X8" ), AlphaBitMask.ToString( "X8" ) );
        }

        //Not a format, but signifies if there exists an extended header with DXGI format enum
        public static readonly PixelFormat DX10Extended = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'D', 'X', '1', '0' ) );

        #region Common Legacy Formats

        //Most of these we'll write out to maximize compatability with old tools (unless noted). According to MSDN, old D3D format names should be read right to left (least to most significant bits).

        public static readonly PixelFormat DXT1 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'D', 'X', 'T', '1' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat DXT2 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'D', 'X', 'T', '2' ), 0, 0, 0, 0, 0 ); //Rarely used, maps to DXT3

        public static readonly PixelFormat DXT3 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'D', 'X', 'T', '3' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat DXT4 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'D', 'X', 'T', '4' ), 0, 0, 0, 0, 0 ); //Rarely used, maps to DXT5

        public static readonly PixelFormat DXT5 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'D', 'X', 'T', '5' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat BC4_UNorm = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'B', 'C', '4', 'U' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat BC4_SNorm = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'B', 'C', '4', 'S' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat BC5_UNorm = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'B', 'C', '5', 'U' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat BC5_SNorm = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'B', 'C', '5', 'S' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat R8G8_B8G8 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'R', 'G', 'B', 'G' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat G8R8_G8B8 = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 'G', 'R', 'G', 'B' ), 0, 0, 0, 0, 0 );

        public static readonly PixelFormat A8R8G8B8 = new PixelFormat( PixelFormatFlags.RGBA, 0, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000 );

        public static readonly PixelFormat X8R8G8B8 = new PixelFormat( PixelFormatFlags.RGB, 0, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000 );

        public static readonly PixelFormat A8B8G8R8 = new PixelFormat( PixelFormatFlags.RGBA, 0, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000 );

        public static readonly PixelFormat X8B8G8R8 = new PixelFormat( PixelFormatFlags.RGB, 0, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0x00000000 ); //Use DX10, Maps to DXGI R8G8B8A8_UNorm

        public static readonly PixelFormat G16R16 = new PixelFormat( PixelFormatFlags.RGB, 0, 32, 0x0000ffff, 0xffff0000, 0x00000000, 0x00000000 );

        public static readonly PixelFormat R5G6B5 = new PixelFormat( PixelFormatFlags.RGB, 0, 16, 0x0000f800, 0x000007e0, 0x0000001f, 0x00000000 );

        public static readonly PixelFormat A1R5G5B5 = new PixelFormat( PixelFormatFlags.RGBA, 0, 16, 0x00007c00, 0x000003e0, 0x0000001f, 0x00008000 );

        public static readonly PixelFormat A4R4G4B4 = new PixelFormat( PixelFormatFlags.RGBA, 0, 16, 0x00000f00, 0x000000f0, 0x0000000f, 0x0000f000 );

        public static readonly PixelFormat R8G8B8 = new PixelFormat( PixelFormatFlags.RGB, 0, 24, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000 ); //Use DX10, Maps to R8G8B8A8_UNorm with opaque alpha

        public static readonly PixelFormat L8 = new PixelFormat( PixelFormatFlags.Luminance, 0, 8, 0xff, 0x00, 0x00, 0x00 );

        public static readonly PixelFormat L16 = new PixelFormat( PixelFormatFlags.Luminance, 0, 16, 0xffff, 0x0000, 0x0000, 0x0000 );

        public static readonly PixelFormat A8L8 = new PixelFormat( PixelFormatFlags.LuminanceAlpha, 0, 16, 0x00ff, 0x0000, 0x0000, 0xff00 );

        public static readonly PixelFormat A8 = new PixelFormat( PixelFormatFlags.Alpha, 0, 8, 0x00, 0x00, 0x00, 0xff );


        //Some common legacy D3DX formats that use D3DFMT enum value as FourCC. The names correspond to the DXGI format name.

        public static readonly PixelFormat R32G32B32A32_Float = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 116 ) ); // D3DFMT_A32B32G32R32F

        public static readonly PixelFormat R16G16B16A16_Float = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 113 ) ); // D3DFMT_A16B16G16R16F

        public static readonly PixelFormat R16G16B16A16_UNorm = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 36 ) ); // D3DFMT_A16B16G16R16

        public static readonly PixelFormat R16G16B16A16_SNorm = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 110 ) ); // D3DFMT_Q16W16V16U16

        public static readonly PixelFormat R32G32_Float = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 115 ) ); // D3DFMT_G32R32F

        public static readonly PixelFormat R16G16_Float = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 112 ) ); // D3DFMT_G16R16F

        public static readonly PixelFormat R32_Float = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 114 ) ); // D3DFMT_R32F

        public static readonly PixelFormat R16_Float = new PixelFormat( PixelFormatFlags.FourCC, new FourCC( 111 ) ); // D3DFMT_R16F

        #endregion
    }

    [StructLayout( LayoutKind.Sequential, Size = 4 )]
    internal struct FourCC : IEquatable<FourCC> {
        private static readonly FourCC s_empty = new FourCC( 0 );
        private static readonly int s_sizeInBytes = TeximpNet.MemoryHelper.SizeOf<FourCC>();

        private uint m_packedValue;

        /// <summary>
        /// Gets the empty (a value of zero) four character code.
        /// </summary>
        public static FourCC Empty {
            get {
                return s_empty;
            }
        }

        /// <summary>
        /// Gets the size of the <see cref="FourCC"/> structure in bytes.
        /// </summary>
        public static int SizeInBytes {
            get {
                return s_sizeInBytes;
            }
        }

        /// <summary>
        /// Gets the first character.
        /// </summary>
        public char First {
            get {
                return ( char )( m_packedValue & 255 );
            }
        }

        /// <summary>
        /// Gets the second character.
        /// </summary>
        public char Second {
            get {
                return ( char )( ( m_packedValue >> 8 ) & 255 );
            }
        }

        /// <summary>
        /// Gets the third character.
        /// </summary>
        public char Third {
            get {
                return ( char )( ( m_packedValue >> 16 ) & 255 );
            }
        }

        /// <summary>
        /// Gets the fourth character.
        /// </summary>
        public char Fourth {
            get {
                return ( char )( ( m_packedValue >> 24 ) & 255 );
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="fourCharacterCode">The string representation of a four character code.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the string representing a four character code does not infact have four characters.</exception>
        public FourCC( String fourCharacterCode ) {
            if( fourCharacterCode != null ) {
                if( fourCharacterCode.Length != 4 )
                    throw new ArgumentOutOfRangeException( "fourCharacterCode", "FourCC must have four characters only." );

                m_packedValue = ( uint )( ( fourCharacterCode[3] << 24 ) | ( fourCharacterCode[2] << 16 ) | ( fourCharacterCode[1] << 8 ) | fourCharacterCode[0] );
            }
            else {
                m_packedValue = 0;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="first">First character</param>
        /// <param name="second">Second character</param>
        /// <param name="third">Third character</param>
        /// <param name="fourth">Fourth character</param>
        public FourCC( char first, char second, char third, char fourth ) {
            m_packedValue = ( uint )( ( ( ( fourth << 24 ) | ( third << 16 ) ) | ( second << 8 ) ) | first );
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="packedValue">Packed value represent the four character code.</param>
        public FourCC( uint packedValue ) {
            m_packedValue = packedValue;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="FourCC"/> struct.
        /// </summary>
        /// <param name="packedValue">Packed value represent the four character code.</param>
        public FourCC( int packedValue ) {
            m_packedValue = ( uint )packedValue;
        }

        /// <summary>
        /// Implicitly converts the <see cref="FourCC"/> instance to an unsigned integer.
        /// </summary>
        /// <param name="fourCharacterCode">Character code</param>
        /// <returns>Unsigned integer representation.</returns>
        public static implicit operator uint( FourCC fourCharacterCode ) {
            return fourCharacterCode.m_packedValue;
        }

        /// <summary>
        /// Implicitly converts the <see cref="FourCC"/> instance to an integer.
        /// </summary>
        /// <param name="fourCharacterCode">Character code</param>
        /// <returns>Integer representation</returns>
        public static implicit operator int( FourCC fourCharacterCode ) {
            return ( int )fourCharacterCode.m_packedValue;
        }

        /// <summary>
        /// Implicitly converts the <see cref="FourCC"/> instance to a String.
        /// </summary>
        /// <param name="fourCharacterCode">Character code</param>
        /// <returns>String representation</returns>
        public static implicit operator String( FourCC fourCharacterCode ) {
            return new String( new char[] { fourCharacterCode.First, fourCharacterCode.Second, fourCharacterCode.Third, fourCharacterCode.Fourth } );
        }

        /// <summary>
        /// Implicitly converts an unsigned integer to a <see cref="FourCC"/> instance.
        /// </summary>
        /// <param name="packedValue">Packed value representing the four character code.</param>
        /// <returns>The FourCC instance.</returns>
        public static implicit operator FourCC( uint packedValue ) {
            return new FourCC( packedValue );
        }

        /// <summary>
        /// Implicitly converts an integer to a <see cref="FourCC"/> instance.
        /// </summary>
        /// <param name="packedValue">Packed value representing the four character code.</param>
        /// <returns>The FourCC instance.</returns>
        public static implicit operator FourCC( int packedValue ) {
            return new FourCC( packedValue );
        }

        /// <summary>
        /// Implicitly converts a String to a <see cref="FourCC"/> instance.
        /// </summary>
        /// <param name="fourCharacterCode">String representing the four character code.</param>
        /// <returns>The FourCC instance.</returns>
        public static implicit operator FourCC( String fourCharacterCode ) {
            return new FourCC( fourCharacterCode );
        }

        /// <summary>
        /// Tests equality between two character codes.
        /// </summary>
        /// <param name="a">First character code</param>
        /// <param name="b">Second character code</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public static bool operator ==( FourCC a, FourCC b ) {
            return a.m_packedValue == b.m_packedValue;
        }

        /// <summary>
        /// Tests inequality between two character codes.
        /// </summary>
        /// <param name="a">First character code</param>
        /// <param name="b">Second character code</param>
        /// <returns>True if both are not equal, false otherwise.</returns>
        public static bool operator !=( FourCC a, FourCC b ) {
            return a.m_packedValue != b.m_packedValue;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>True if the specified <see cref="System.Object" /> is equal to this instance; otherwise, false.</returns>
        public override bool Equals( object obj ) {
            if( obj is FourCC )
                return Equals( ( FourCC )obj );

            return false;
        }

        /// <summary>
        /// Tests equality between this character code and another.
        /// </summary>
        /// <param name="other">Other character code</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals( FourCC other ) {
            return m_packedValue == other.m_packedValue;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return ( int )m_packedValue;
            }
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
        public override String ToString() {
            if( m_packedValue == 0 )
                return "0";

            return new String( new char[] { First, Second, Third, Fourth } );
        }
    }
}
