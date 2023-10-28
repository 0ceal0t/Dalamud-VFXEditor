using Lumina.Data.Parsing.Tex;
using System;
using System.IO;
using System.Runtime.InteropServices;
using TeximpNet;
using TeximpNet.Compression;
using TeximpNet.DDS;
using VfxEditor.FileBrowser;
using VfxEditor.Utils;

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
        TextureTypeCube = 0x2000000,
        TextureTypeMask = 0x3C00000,
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
    }

    public class TextureDataFile : Lumina.Data.FileResource {
        [StructLayout( LayoutKind.Sequential, Pack = 4 )]
        public unsafe struct TexHeader {
            public Attribute Type;
            public TextureFormat Format;
            public ushort Width;
            public ushort Height;
            public ushort Depth;
            public ushort MipLevels;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
            public uint[] LodOffset;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 13 )]
            public uint[] OffsetToSurface;
        };

        public bool ValidFormat { get; private set; } = false;
        public byte[] ImageData { get; private set; } // decompressed into ARGB or whatever. used for image previews
        public TexHeader Header { get; private set; }

        private static int HeaderLength => Marshal.SizeOf( typeof( TexHeader ) );
        private byte[] DdsData;

        public bool Local { get; private set; } = false; // was this loaded from the game using Lumina, or from a local ATEX?
        private byte[] LocalData;

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;

            var buffer = Reader.ReadBytes( HeaderLength );
            var handle = Marshal.AllocHGlobal( HeaderLength );
            Marshal.Copy( buffer, 0, handle, HeaderLength );
            Header = ( TexHeader )Marshal.PtrToStructure( handle, typeof( TexHeader ) );
            Marshal.FreeHGlobal( handle );

            ImageData = BgraToRgba( Convert( DataSpan[HeaderLength..].ToArray(), Header.Format, Header.Width, Header.Height ) );
            ValidFormat = ImageData.Length > 0;
        }

        public void LoadFile( byte[] localData ) {
            LocalData = localData;
            using var ms = new MemoryStream( LocalData );
            using var br = new BinaryReader( ms );

            Local = true;
            br.BaseStream.Position = 0;

            var buffer = br.ReadBytes( HeaderLength );
            var handle = Marshal.AllocHGlobal( HeaderLength );
            Marshal.Copy( buffer, 0, handle, HeaderLength );
            Header = ( TexHeader )Marshal.PtrToStructure( handle, typeof( TexHeader ) );
            Marshal.FreeHGlobal( handle );

            DdsData = br.ReadBytes( localData.Length - HeaderLength );
            ImageData = BgraToRgba( Convert( DdsData, Header.Format, Header.Width, Header.Height ) );
            ValidFormat = ImageData.Length > 0;
        }

        public static TextureDataFile LoadFromLocal( string path ) {
            var tex = new TextureDataFile();
            tex.LoadFile( File.ReadAllBytes( path ) );
            return tex;
        }

        // converts various formats to A8R8G8B8
        public static byte[] Convert( byte[] data, TextureFormat format, int width, int height ) {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter( ms );

            switch( format ) {
                case TextureFormat.DXT1:
                    DecompressDxt1( data, writer, width, height );
                    break;
                case TextureFormat.DXT3:
                    DecompressDxt3( data, writer, width, height );
                    break;
                case TextureFormat.DXT5:
                    DecompressDxt5( data, writer, width, height );
                    break;
                case TextureFormat.A8R8G8B8:
                    writer.Write( data ); // already ok
                    break;
                case TextureFormat.R4G4B4A4:
                    Read4444( data, writer, width, height );
                    break;
                case TextureFormat.R5G5B5A1:
                    Read5551( data, writer, width, height );
                    break;
                case TextureFormat.A8:
                    DecompressA8( data, writer, width, height );
                    break;
                default:
                    return Array.Empty<byte>(); // ???
            }

            var output = ms.ToArray();
            var result = new byte[width * height * 4];
            Array.Copy( output, result, Math.Min( output.Length, result.Length ) );

            return result;
        }

        public static TextureFormat DXGItoTextureFormat( DXGIFormat format ) {
            return format switch {
                DXGIFormat.A8_UNorm => TextureFormat.A8,
                DXGIFormat.BC1_UNorm => TextureFormat.DXT1,
                DXGIFormat.BC2_UNorm => TextureFormat.DXT3,
                DXGIFormat.BC3_UNorm => TextureFormat.DXT5,
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
                TextureFormat.A8R8G8B8 or TextureFormat.R4G4B4A4 or TextureFormat.A8 or TextureFormat.R5G5B5A1 => CompressionFormat.BGRA,
                _ => CompressionFormat.ETC1,
            };
        }

        public static byte[] CompressA8( byte[] data ) {
            // r g b A r g b A, ... only take the A part
            var ret = new byte[data.Length / 4];
            for( var i = 0; i < ret.Length; i++ ) {
                ret[i] = data[i * 4 + 3];
            }
            return ret;
        }

        public static void DecompressA8( byte[] data, BinaryWriter writer, int width, int height ) {
            for( var i = 0; i < width * height; i++ ) {
                writer.Write( ( byte )0xFF );
                writer.Write( ( byte )0xFF );
                writer.Write( ( byte )0xFF );
                writer.Write( data[i] );
            }
        }

        public static void DecompressDxt1( byte[] data, BinaryWriter writer, int width, int height ) {
            writer.Write( Squish.DecompressImage( data, width, height, SquishOptions.DXT1 ) );
        }

        public static void DecompressDxt3( byte[] data, BinaryWriter writer, int width, int height ) {
            writer.Write( Squish.DecompressImage( data, width, height, SquishOptions.DXT3 ) );
        }

        public static void DecompressDxt5( byte[] data, BinaryWriter writer, int width, int height ) {
            writer.Write( Squish.DecompressImage( data, width, height, SquishOptions.DXT5 ) );
        }

        public static void Read4444( byte[] src, BinaryWriter writer, int width, int height ) {
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

        public static void Read5551( byte[] src, BinaryWriter writer, int width, int height ) {
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

        // ==================

        public byte[] GetAllData() => Local ? LocalData : Data;

        public byte[] GetDdsData() => Local ? DdsData : DataSpan[HeaderLength..].ToArray();

        public void SaveAsPng( string path ) {
            var data = new RGBAQuad[Header.Height * Header.Width];
            for( var i = 0; i < Header.Height; i++ ) {
                for( var j = 0; j < Header.Width; j++ ) {
                    var idx = i * Header.Width + j;
                    data[idx] = new RGBAQuad( ImageData[idx * 4], ImageData[idx * 4 + 1], ImageData[idx * 4 + 2], ImageData[idx * 4 + 3] );
                }
            }
            var ptr = MemoryHelper.PinObject( data );
            var image = Surface.LoadFromRawData( ptr, Header.Width, Header.Height, Header.Width * 4, false, true );
            if( image == null ) return;

            image.SaveToFile( ImageFormat.PNG, path );
        }

        public void SaveAsDds( string path ) {
            var header = TextureUtils.CreateDdsHeader( Header.Width, Header.Height, Header.Format, Header.Depth, Header.MipLevels );
            var data = GetDdsData();
            var writeData = new byte[header.Length + data.Length];
            Buffer.BlockCopy( header, 0, writeData, 0, header.Length );
            Buffer.BlockCopy( data, 0, writeData, header.Length, data.Length );
            File.WriteAllBytes( path, writeData );
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
