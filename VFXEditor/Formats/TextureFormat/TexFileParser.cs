using Lumina.Data.Files;
using Lumina.Extensions;
using OtterTex;
using System;
using System.IO;

namespace VfxEditor.Formats.TextureFormat {
    public static class TexFileParser {
        public static ScratchImage Parse( Stream data ) {
            using var reader = new BinaryReader( data );
            var header = reader.ReadStructure<TexFile.TexHeader>();
            var isArray = header.Type == TexFile.Attribute.TextureType2DArray;

            var meta = header.ToTexMeta();
            if( meta.Format == DXGIFormat.Unknown )
                throw new InvalidDataException( $"Could not convert format {header.Format} to DXGI Format." );

            if( meta.Dimension == TexDimension.Unknown && !isArray )
                throw new InvalidDataException( $"Could not obtain dimensionality from {header.Type} / {header.Format}." );

            meta.MipLevels = CountMipLevels( data, in meta, in header );
            if( meta.MipLevels == 0 )
                throw new InvalidDataException( "Could not load file. Image is corrupted and does not contain enough data for its size." );

            if( isArray ) {
                // https://github.com/nadlabka/DX11Renderer/blob/abe3e89ad3b8f0ac8ba8fa42ef1bc6c69d4033aa/Engine/DXRenderer/Managers/TexturesManager.cpp#L119
                var scratch = ScratchImage.Initialize2D( meta.Format, header.Width, header.Height, header.ArraySize, 1 );
                // TODO: fix this
                for( var i = 0; i < header.ArraySize; i++ ) {
                    var image = scratch.GetImage( 0, i, 0 );
                    CopyData( image.Span, reader );
                }
                return scratch;

            }
            else {
                var scratch = ScratchImage.Initialize( meta );
                CopyData( scratch.Pixels, reader );
                return scratch;
            }
        }

        private static unsafe int CountMipLevels( Stream data, in TexMeta meta, in TexFile.TexHeader header ) {
            var width = meta.Width;
            var height = meta.Height;
            var depth = meta.Depth;

            var lastOffset = 0L;
            var lastSize = 80L;
            for( var i = 0; i < 13; ++i ) {
                var offset = header.OffsetToSurface[i];
                if( offset == 0 )
                    return i;

                var Size = ( long )meta.Format.ComputePitch( width, height ).Item2 * depth;
                if( offset + Size > data.Length )
                    return i;

                var diff = offset - lastOffset;
                if( diff != lastSize )
                    return i;

                width = Math.Max( width / 2, 1 );
                height = Math.Max( height / 2, 1 );
                depth = Math.Max( depth / 2, 1 );
                lastOffset = offset;
                lastSize = Size;
            }

            return 13;
        }

        public static unsafe void FixMipOffsets( long size, ref TexFile.TexHeader header, out long newSize ) {
            var width = ( uint )header.Width;
            var height = ( uint )header.Height;
            var format = header.Format.ToDXGI();
            var bits = format.BitsPerPixel();
            var totalSize = 80u;
            size -= totalSize;
            var minSize = format.IsCompressed() ? 4u : 1u;
            for( var i = 0; i < 13; ++i ) {
                var requiredSize = ( uint )( ( long )width * height * bits / 8 );
                if( requiredSize > size ) {
                    newSize = totalSize;
                    if( header.MipCount != i ) {
                        FixLodOffsets( ref header, i );
                    }

                    return;
                }

                if( header.OffsetToSurface[i] != totalSize ) {
                    header.OffsetToSurface[i] = totalSize;
                }

                if( width == minSize && height == minSize ) {
                    ++i;
                    newSize = totalSize + requiredSize;
                    if( header.MipCount != i ) {
                        FixLodOffsets( ref header, i );
                    }

                    return;
                }

                totalSize += requiredSize;
                size -= requiredSize;
                width = Math.Max( width / 2, minSize );
                height = Math.Max( height / 2, minSize );
            }

            newSize = totalSize;
            if( header.MipCount != 13 ) {
                FixLodOffsets( ref header, 13 );
            }

            static void FixLodOffsets( ref TexFile.TexHeader header, int index ) {
                header.MipCount = index;
                if( header.LodOffset[2] >= header.MipCount )
                    header.LodOffset[2] = ( byte )( header.MipCount - 1 );
                if( header.LodOffset[1] >= header.MipCount )
                    header.LodOffset[1] = header.MipCount > 2 ? ( byte )( header.MipCount - 2 ) : ( byte )( header.MipCount - 1 );
                for( ++index; index < 13; ++index )
                    header.OffsetToSurface[index] = 0;
            }
        }

        private static unsafe void CopyData( ReadOnlySpan<byte> bytes, BinaryReader r ) {
            fixed( byte* ptr = bytes ) {
                var span = new Span<byte>( ptr, bytes.Length );
                var readBytes = r.Read( span );
                if( readBytes < bytes.Length )
                    throw new InvalidDataException( $"Invalid data length {readBytes} < {bytes.Length}." );
            }
        }

        public static void Write( this TexFile.TexHeader header, BinaryWriter w ) {
            w.Write( ( uint )header.Type );
            w.Write( ( uint )header.Format );
            w.Write( header.Width );
            w.Write( header.Height );
            w.Write( header.Depth );
            w.Write( ( byte )( header.MipCount | ( header.MipUnknownFlag ? 0x80 : 0 ) ) );
            w.Write( header.ArraySize );
            unsafe {
                w.Write( header.LodOffset[0] );
                w.Write( header.LodOffset[1] );
                w.Write( header.LodOffset[2] );
                for( var i = 0; i < 13; ++i )
                    w.Write( header.OffsetToSurface[i] );
            }
        }

        public static TexFile.TexHeader ToTexHeader( this ScratchImage scratch ) => scratch.ToTexHeader( out _ );

        public static TexFile.TexHeader ToTexHeader( this ScratchImage scratch, out int dataLength ) {
            var meta = scratch.Meta;
            var ret = new TexFile.TexHeader() {
                Height = ( ushort )meta.Height,
                Width = ( ushort )meta.Width,
                Depth = ( ushort )Math.Max( meta.Depth, 1 ),
                MipCount = ( byte )Math.Min( meta.MipLevels, 13 ),
                Format = meta.Format.ToTexFormat(),
                Type = meta.Dimension switch {
                    _ when meta.IsCubeMap => TexFile.Attribute.TextureTypeCube,
                    TexDimension.Tex1D => TexFile.Attribute.TextureType1D,
                    TexDimension.Tex2D => TexFile.Attribute.TextureType2D,
                    TexDimension.Tex3D => TexFile.Attribute.TextureType3D,
                    _ => 0,
                },
            };

            ret.FillSurfaceOffsets( scratch, out dataLength );

            return ret;
        }

        private static unsafe void FillSurfaceOffsets( this ref TexFile.TexHeader header, ScratchImage scratch, out int dataLength ) {
            var meta = scratch.Meta;
            var mipCount = Math.Min( meta.MipLevels, 13 );
            var images = scratch.Images;
            dataLength = scratch.Pixels.Length;

            fixed( byte* ptr = scratch.Pixels ) {
                for( var mip = 0; mip < mipCount; mip++ ) {
                    var image = images[meta.ComputeIndex( mip, 0, 0 )];
                    header.OffsetToSurface[mip] = ( uint )( 80 + ( ( byte* )image.Pixels - ptr ) );
                }

                // Drop data for any mips past the 13 the header can describe
                if( meta.MipLevels > 13 ) dataLength = ( int )( ( byte* )images[meta.ComputeIndex( 13, 0, 0 )].Pixels - ptr );
            }

            for( var idx = mipCount; idx < 13; ++idx )
                header.OffsetToSurface[idx] = 0;

            header.LodOffset[0] = 0;
            header.LodOffset[1] = ( byte )Math.Min( 1, mipCount - 1 );
            header.LodOffset[2] = ( byte )Math.Min( 2, mipCount - 1 );
        }

        public static int MaxMipLevels( int width, int height, DXGIFormat format ) {
            var minSize = format.IsCompressed() ? 4 : 1;
            var count = 1;
            while( count < 13 && ( width > minSize || height > minSize ) ) {
                width = Math.Max( width / 2, 1 );
                height = Math.Max( height / 2, 1 );
                count++;
            }
            return count;
        }

        public static void SaveTex( ScratchImage scratch, string path ) {
            var meta = scratch.Meta;
            if( meta.IsCubeMap || meta.ArraySize > 1 )
                throw new NotSupportedException( "Cube map and texture array import is not supported" );

            var header = scratch.ToTexHeader( out var dataLength );
            if( header.Format == TexFile.TextureFormat.Unknown )
                throw new NotSupportedException( $"Format {meta.Format} cannot be stored in a .tex file" );

            using var stream = File.Open( path, FileMode.Create );
            using var writer = new BinaryWriter( stream );
            header.Write( writer );
            writer.Write( scratch.Pixels[..dataLength] );
            // The span is only valid while the scratch image is alive
            GC.KeepAlive( scratch );
        }


        public static TexMeta ToTexMeta( this TexFile.TexHeader header )
            => new() {
                Height = header.Height,
                Width = header.Width,
                Depth = Math.Max( header.Depth, ( ushort )1 ),
                MipLevels = header.MipCount,
                ArraySize = 1,
                Format = header.Format.ToDXGI(),
                Dimension = header.Type.ToDimension(),
                MiscFlags = header.Type.HasFlag( TexFile.Attribute.TextureTypeCube ) ? D3DResourceMiscFlags.TextureCube : 0,
                MiscFlags2 = 0,
            };

        private static TexDimension ToDimension( this TexFile.Attribute attribute )
            => ( attribute & TexFile.Attribute.TextureTypeMask ) switch {
                TexFile.Attribute.TextureType1D => TexDimension.Tex1D,
                TexFile.Attribute.TextureType2D => TexDimension.Tex2D,
                TexFile.Attribute.TextureType3D => TexDimension.Tex3D,
                _ => TexDimension.Unknown,
            };

        public static TexFile.TextureFormat ToTexFormat( this DXGIFormat format )
            => format switch {
                DXGIFormat.R8UNorm => TexFile.TextureFormat.L8,
                DXGIFormat.A8UNorm => TexFile.TextureFormat.A8,
                DXGIFormat.B4G4R4A4UNorm => TexFile.TextureFormat.B4G4R4A4,
                DXGIFormat.B5G5R5A1UNorm => TexFile.TextureFormat.B5G5R5A1,
                DXGIFormat.B8G8R8A8UNorm => TexFile.TextureFormat.B8G8R8A8,
                DXGIFormat.B8G8R8X8UNorm => TexFile.TextureFormat.B8G8R8X8,
                DXGIFormat.R32Float => TexFile.TextureFormat.R32F,
                DXGIFormat.R16G16Float => TexFile.TextureFormat.R16G16F,
                DXGIFormat.R32G32Float => TexFile.TextureFormat.R32G32F,
                DXGIFormat.R16G16B16A16Float => TexFile.TextureFormat.R16G16B16A16F,
                DXGIFormat.R32G32B32A32Float => TexFile.TextureFormat.R32G32B32A32F,
                DXGIFormat.BC1UNorm => TexFile.TextureFormat.BC1,
                DXGIFormat.BC2UNorm => TexFile.TextureFormat.BC2,
                DXGIFormat.BC3UNorm => TexFile.TextureFormat.BC3,
                DXGIFormat.BC4UNorm => ( TexFile.TextureFormat )0x6120, // TODO: upstream to Lumina
                DXGIFormat.BC5UNorm => TexFile.TextureFormat.BC5,
                DXGIFormat.BC6HSF16 => ( TexFile.TextureFormat )0x6330, // TODO: upstream to Lumina
                DXGIFormat.BC7UNorm => TexFile.TextureFormat.BC7,
                DXGIFormat.R16G16B16A16Typeless => TexFile.TextureFormat.D16,
                DXGIFormat.R24G8Typeless => TexFile.TextureFormat.D24S8,
                DXGIFormat.R16Typeless => TexFile.TextureFormat.Shadow16,
                _ => TexFile.TextureFormat.Unknown,
            };

        public static DXGIFormat ToDXGI( this TexFile.TextureFormat format )
            => format switch {
                TexFile.TextureFormat.L8 => DXGIFormat.R8UNorm,
                TexFile.TextureFormat.A8 => DXGIFormat.A8UNorm,
                TexFile.TextureFormat.B4G4R4A4 => DXGIFormat.B4G4R4A4UNorm,
                TexFile.TextureFormat.B5G5R5A1 => DXGIFormat.B5G5R5A1UNorm,
                TexFile.TextureFormat.B8G8R8A8 => DXGIFormat.B8G8R8A8UNorm,
                TexFile.TextureFormat.B8G8R8X8 => DXGIFormat.B8G8R8X8UNorm,
                TexFile.TextureFormat.R32F => DXGIFormat.R32Float,
                TexFile.TextureFormat.R16G16F => DXGIFormat.R16G16Float,
                TexFile.TextureFormat.R32G32F => DXGIFormat.R32G32Float,
                TexFile.TextureFormat.R16G16B16A16F => DXGIFormat.R16G16B16A16Float,
                TexFile.TextureFormat.R32G32B32A32F => DXGIFormat.R32G32B32A32Float,
                TexFile.TextureFormat.BC1 => DXGIFormat.BC1UNorm,
                TexFile.TextureFormat.BC2 => DXGIFormat.BC2UNorm,
                TexFile.TextureFormat.BC3 => DXGIFormat.BC3UNorm,
                ( TexFile.TextureFormat )0x6120 => DXGIFormat.BC4UNorm, // TODO: upstream to Lumina
                TexFile.TextureFormat.BC5 => DXGIFormat.BC5UNorm,
                ( TexFile.TextureFormat )0x6330 => DXGIFormat.BC6HSF16, // TODO: upstream to Lumina
                TexFile.TextureFormat.BC7 => DXGIFormat.BC7UNorm,
                TexFile.TextureFormat.D16 => DXGIFormat.R16G16B16A16Typeless,
                TexFile.TextureFormat.D24S8 => DXGIFormat.R24G8Typeless,
                TexFile.TextureFormat.Shadow16 => DXGIFormat.R16Typeless,
                TexFile.TextureFormat.Shadow24 => DXGIFormat.R24G8Typeless,
                _ => DXGIFormat.Unknown,
            };
    }
}