using System.Collections.Generic;
using System.IO;
using TeximpNet;
using TeximpNet.DDS;

namespace VfxEditor.Formats.TextureFormat.CustomTeximpNet {
    internal class CustomDDSFile {
        public static bool Write( Stream output, List<MipChain> mipChains, DXGIFormat format, TextureDimension texDim, DDSFlags flags = DDSFlags.None ) {
            if( output == null || !output.CanWrite || mipChains == null || mipChains.Count == 0 || mipChains[0].Count == 0 || format == DXGIFormat.Unknown )
                return false;

            //Extract details
            int width, height, depth, arrayCount, mipCount;
            var firstMip = mipChains[0][0];
            width = firstMip.Width;
            height = firstMip.Height;
            depth = firstMip.Depth;
            arrayCount = mipChains.Count;
            mipCount = mipChains[0].Count;

            if( !ValidateInternal( mipChains, format, texDim ) )
                return false;

            var minSize = MemoryHelper.SizeOf<CustomFourCC>() + MemoryHelper.SizeOf<CustomHeader>() + MemoryHelper.SizeOf<CustomHeader10>();

            //Setup a transfer buffer
            var buffer = new StreamTransferBuffer( minSize, false );

            //Write out header
            if( !WriteHeader( output, buffer, texDim, format, width, height, depth, arrayCount, mipCount, flags ) )
                return false;

            if( buffer.Length < firstMip.RowPitch ) buffer.Resize( firstMip.RowPitch, false );

            //Iterate over each array face...
            for( var i = 0; i < arrayCount; i++ ) {
                var mipChain = mipChains[i];

                //Iterate over each mip face...
                for( var mipLevel = 0; mipLevel < mipCount; mipLevel++ ) {
                    var mip = mipChain[mipLevel];

                    //Compute pitch, based on MSDN programming guide. We will write out these pitches rather than the supplied in order to conform to the recomendation
                    //that we compute pitch based on format
                    ImageHelper.ComputePitch( format, mip.Width, mip.Height, out var dstRowPitch, out var dstSlicePitch, out _, out var realMipHeight, out _ );

                    //Ensure write buffer is sufficiently sized for a single scanline
                    if( buffer.Length < dstRowPitch )
                        buffer.Resize( dstRowPitch, false );

                    //Sanity check
                    if( dstRowPitch < mip.RowPitch )
                        return false;

                    var srcPtr = mip.Data;

                    //Advance stream one slice at a time...
                    for( var slice = 0; slice < mip.Depth; slice++ ) {
                        var bytesToWrite = dstSlicePitch;
                        var sPtr = srcPtr;

                        //Copy scanline into temp buffer, write to output
                        for( var row = 0; row < realMipHeight; row++ ) {
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
        }

        public static bool WriteHeader( Stream output, StreamTransferBuffer buffer, TextureDimension texDim, DXGIFormat format, int width, int height, int depth, int arrayCount, int mipCount, DDSFlags flags ) {
            //Force the DX10 header...
            var writeDX10Header = ( flags & DDSFlags.ForceExtendedHeader ) == DDSFlags.ForceExtendedHeader;

            //Or do DX10 if the following is true...1D textures or 2D texture arrays that aren't cubemaps...
            if( !writeDX10Header ) {
                switch( texDim ) {
                    case TextureDimension.One:
                        writeDX10Header = true;
                        break;
                    case TextureDimension.Two:
                        writeDX10Header = arrayCount > 1;
                        break;
                }
            }

            //Figure out pixel format, if not writing DX10 header...
            CustomPixelFormat CustomPixelFormat;
            if( !writeDX10Header ) {
                switch( format ) {
                    case DXGIFormat.R8G8B8A8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.A8B8G8R8;
                        break;
                    case DXGIFormat.R16G16_UNorm:
                        CustomPixelFormat = CustomPixelFormat.G16R16;
                        break;
                    case DXGIFormat.R8G8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.A8L8;
                        break;
                    case DXGIFormat.R16_UNorm:
                        CustomPixelFormat = CustomPixelFormat.L16;
                        break;
                    case DXGIFormat.R8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.L8;
                        break;
                    case DXGIFormat.A8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.A8;
                        break;
                    case DXGIFormat.R8G8_B8G8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.R8G8_B8G8;
                        break;
                    case DXGIFormat.G8R8_G8B8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.G8R8_G8B8;
                        break;
                    case DXGIFormat.BC1_UNorm:
                        CustomPixelFormat = CustomPixelFormat.DXT1;
                        break;
                    case DXGIFormat.BC2_UNorm:
                        CustomPixelFormat = CustomPixelFormat.DXT3;
                        break;
                    case DXGIFormat.BC3_UNorm:
                        CustomPixelFormat = CustomPixelFormat.DXT5;
                        break;
                    case DXGIFormat.BC4_UNorm:
                        CustomPixelFormat = CustomPixelFormat.BC4_UNorm;
                        break;
                    case DXGIFormat.BC4_SNorm:
                        CustomPixelFormat = CustomPixelFormat.BC4_SNorm;
                        break;
                    case DXGIFormat.BC5_UNorm:
                        CustomPixelFormat = CustomPixelFormat.BC5_UNorm;
                        break;
                    case DXGIFormat.BC5_SNorm:
                        CustomPixelFormat = CustomPixelFormat.BC5_SNorm;
                        break;
                    case DXGIFormat.B5G6R5_UNorm:
                        CustomPixelFormat = CustomPixelFormat.R5G6B5;
                        break;
                    case DXGIFormat.B5G5R5A1_UNorm:
                        CustomPixelFormat = CustomPixelFormat.A1R5G5B5;
                        break;
                    case DXGIFormat.B8G8R8A8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.A8R8G8B8;
                        break;
                    case DXGIFormat.B8G8R8X8_UNorm:
                        CustomPixelFormat = CustomPixelFormat.X8R8G8B8;
                        break;
                    case DXGIFormat.B4G4R4A4_UNorm:
                        CustomPixelFormat = CustomPixelFormat.A4R4G4B4;
                        break;
                    case DXGIFormat.R32G32B32A32_Float:
                        CustomPixelFormat = CustomPixelFormat.R32G32B32A32_Float;
                        break;
                    case DXGIFormat.R16G16B16A16_Float:
                        CustomPixelFormat = CustomPixelFormat.R16G16B16A16_Float;
                        break;
                    case DXGIFormat.R16G16B16A16_UNorm:
                        CustomPixelFormat = CustomPixelFormat.R16G16B16A16_UNorm;
                        break;
                    case DXGIFormat.R16G16B16A16_SNorm:
                        CustomPixelFormat = CustomPixelFormat.R16G16B16A16_SNorm;
                        break;
                    case DXGIFormat.R32G32_Float:
                        CustomPixelFormat = CustomPixelFormat.R32G32_Float;
                        break;
                    case DXGIFormat.R16G16_Float:
                        CustomPixelFormat = CustomPixelFormat.R16G16_Float;
                        break;
                    case DXGIFormat.R32_Float:
                        CustomPixelFormat = CustomPixelFormat.R32_Float;
                        break;
                    case DXGIFormat.R16_Float:
                        CustomPixelFormat = CustomPixelFormat.R16_Float;
                        break;
                    default:
                        CustomPixelFormat = CustomPixelFormat.DX10Extended;
                        writeDX10Header = true;
                        break;
                }
            }
            else {
                CustomPixelFormat = CustomPixelFormat.DX10Extended;
            }

            var header = new CustomHeader {
                Size = ( uint )MemoryHelper.SizeOf<CustomHeader>(),
                PixelFormat = CustomPixelFormat,
                Flags = CustomHeaderFlags.Caps | CustomHeaderFlags.Width | CustomHeaderFlags.Height | CustomHeaderFlags.PixelFormat,
                Caps = CustomHeaderCaps.Texture
            };
            CustomHeader10? header10 = null;

            if( mipCount > 0 ) {
                header.Flags |= CustomHeaderFlags.MipMapCount;
                header.MipMapCount = ( uint )mipCount;
                header.Caps |= CustomHeaderCaps.MipMap;
            }

            switch( texDim ) {
                case TextureDimension.One:
                    header.Width = ( uint )width;
                    header.Height = 1;
                    header.Depth = 1;

                    //Should always be writing out extended header for 1D textures
                    System.Diagnostics.Debug.Assert( writeDX10Header );

                    header10 = new( format, CustomD3D10ResourceDimension.Texture1D, CustomHeader10Flags.None, ( uint )arrayCount, CustomHeader10Flags2.None );

                    break;
                case TextureDimension.Two:
                    header.Width = ( uint )width;
                    header.Height = ( uint )height;
                    header.Depth = 1;

                    if( writeDX10Header )
                        header10 = new( format, CustomD3D10ResourceDimension.Texture2D, CustomHeader10Flags.None, ( uint )arrayCount, CustomHeader10Flags2.None );

                    break;
                case TextureDimension.Cube:
                    header.Width = ( uint )width;
                    header.Height = ( uint )height;
                    header.Depth = 1;
                    header.Caps |= CustomHeaderCaps.Complex;
                    header.Caps2 |= CustomHeaderCaps2.Cubemap_AllFaces;

                    //can support array tex cubes, so must be multiples of 6
                    if( arrayCount % 6 != 0 )
                        return false;

                    if( writeDX10Header )
                        header10 = new( format, CustomD3D10ResourceDimension.Texture2D, CustomHeader10Flags.TextureCube, ( uint )arrayCount / 6, CustomHeader10Flags2.None );

                    break;
                case TextureDimension.Three:
                    header.Width = ( uint )width;
                    header.Height = ( uint )height;
                    header.Depth = ( uint )depth;
                    header.Flags |= CustomHeaderFlags.Depth;
                    header.Caps2 |= CustomHeaderCaps2.Volume;

                    if( arrayCount != 1 )
                        return false;

                    if( writeDX10Header )
                        header10 = new( format, CustomD3D10ResourceDimension.Texture3D, CustomHeader10Flags.None, 1, CustomHeader10Flags2.None );

                    break;
            }

            ImageHelper.ComputePitch( format, width, height, out var rowPitch, out var slicePitch, out var _, out var _ );

            if( IsCompressed( format ) ) {
                header.Flags |= CustomHeaderFlags.LinearSize;
                header.PitchOrLinearSize = ( uint )slicePitch;
            }
            else {
                header.Flags |= CustomHeaderFlags.Pitch;
                header.PitchOrLinearSize = ( uint )rowPitch;
            }

            //Write out magic word, DDS header, and optionally extended header

            buffer.Write( output, DDS_MAGIC );
            buffer.Write( output, header );

            if( header10.HasValue ) {
                System.Diagnostics.Debug.Assert( header.PixelFormat.IsDX10Extended );
                buffer.Write( output, header10.Value );
            }

            return true;
        }

        public static bool IsCompressed( DXGIFormat format ) {
            return format switch {
                DXGIFormat.BC1_Typeless or
                DXGIFormat.BC1_UNorm or
                DXGIFormat.BC1_UNorm_SRGB or
                DXGIFormat.BC2_Typeless or
                DXGIFormat.BC2_UNorm or
                DXGIFormat.BC2_UNorm_SRGB or
                DXGIFormat.BC3_Typeless or
                DXGIFormat.BC3_UNorm or
                DXGIFormat.BC3_UNorm_SRGB or
                DXGIFormat.BC4_Typeless or
                DXGIFormat.BC4_UNorm or
                DXGIFormat.BC4_SNorm or
                DXGIFormat.BC5_Typeless or
                DXGIFormat.BC5_UNorm or
                DXGIFormat.BC5_SNorm or
                DXGIFormat.BC6H_Typeless or
                DXGIFormat.BC6H_UF16 or
                DXGIFormat.BC6H_SF16 or
                DXGIFormat.BC7_Typeless or
                DXGIFormat.BC7_UNorm or
                DXGIFormat.BC7_UNorm_SRGB => true,
                _ => false,
            };
        }

        private static readonly CustomFourCC DDS_MAGIC = new( 'D', 'D', 'S', ' ' );

        internal static bool ValidateInternal( List<MipChain> mipChains, DXGIFormat format, TextureDimension texDim ) {
            if( format == DXGIFormat.Unknown )
                return false;

            //Mipchains must exist, must have at least one, and chain must have mipmaps.
            if( mipChains == null || mipChains.Count == 0 || mipChains[0].Count == 0 )
                return false;

            //Validate cubemap...must have multiples of 6 faces (can be an array of cubes).
            if( texDim == TextureDimension.Cube && mipChains.Count % 6 != 0 )
                return false;

            //Validate 3d texture..can't have arrays
            if( texDim == TextureDimension.Three && mipChains.Count > 1 )
                return false;

            int width, height, depth, rowPitch, slicePitch;

            //Save the first image dimensions
            var firstSurface = mipChains[0][0];
            width = firstSurface.Width;
            height = firstSurface.Height;
            depth = firstSurface.Depth;
            rowPitch = firstSurface.RowPitch;
            slicePitch = firstSurface.SlicePitch;

            //Validate first surface
            if( width < 1 || height < 1 || depth < 1 || rowPitch < 1 || slicePitch < 1 )
                return false;

            //Validate 1D texture...must only have 1 height
            if( texDim == TextureDimension.One && height > 1 )
                return false;

            //Validate cubemap...width/height must be same
            if( texDim == TextureDimension.Cube && width != height )
                return false;

            //Only 3d textures have depth
            if( texDim != TextureDimension.Three && depth > 1 )
                return false;

            //Go through each chain and validate against the first texture and ensure mipmaps are progressively smaller
            var mipCount = -1;

            for( var i = 0; i < mipChains.Count; i++ ) {
                var mipmaps = mipChains[i];

                //Mips must exist...
                if( mipmaps == null || mipmaps.Count == 0 )
                    return false;

                //Grab a mip count from first chain
                if( mipCount == -1 )
                    mipCount = mipmaps.Count;

                //Each chain must have the same number of mip surfaces
                if( mipmaps.Count != mipCount )
                    return false;

                //Each mip surface must have data and check sizes
                var prevMip = mipmaps[0];

                //Check against the first main image we looked at earlier
                if( prevMip.Width != width || prevMip.Height != height || prevMip.Depth != depth || prevMip.Data == nint.Zero || prevMip.RowPitch != rowPitch || prevMip.SlicePitch != slicePitch )
                    return false;

                for( var mipLevel = 1; mipLevel < mipmaps.Count; mipLevel++ ) {
                    var nextMip = mipmaps[mipLevel];

                    //Ensure each mipmap is progressively smaller or same at the least
                    if( nextMip.Width > prevMip.Width || nextMip.Height > prevMip.Height || nextMip.Depth > prevMip.Depth || nextMip.Data == nint.Zero
                        || nextMip.RowPitch > prevMip.RowPitch || nextMip.SlicePitch > prevMip.SlicePitch || nextMip.RowPitch == 0 || nextMip.SlicePitch == 0 )
                        return false;

                    prevMip = nextMip;
                }
            }

            return true;
        }
    }
}
