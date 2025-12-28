using Lumina.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private OtterTex.ScratchImage ScratchImage;
        public List<byte[]> Layers { get; private set; }
        public byte[] ImageData => Layers[0];

        private static int HeaderLength => Marshal.SizeOf<TexHeader>();
        private byte[] AllData;
        private byte[] DdsData;

        public override void LoadFile() => LoadFile( Reader );

        public static TextureDataFile LoadFromLocal( string path ) {
            var tex = new TextureDataFile();
            using var ms = new MemoryStream( File.ReadAllBytes( path ) );
            using var reader = new BinaryReader( ms );
            tex.LoadFile( reader );
            return tex;
        }

        private void LoadFile( BinaryReader reader ) {
            reader.BaseStream.Position = 0;
            AllData = reader.ReadBytes( ( int )reader.BaseStream.Length );
            reader.BaseStream.Position = 0;

            var buffer = reader.ReadBytes( HeaderLength );
            var handle = Marshal.AllocHGlobal( HeaderLength );
            Marshal.Copy( buffer, 0, handle, HeaderLength );
            Header = Marshal.PtrToStructure<TexHeader>( handle );
            Marshal.FreeHGlobal( handle );

            DdsData = reader.ReadBytes( AllData.Length - HeaderLength );

            using var ms = new MemoryStream( GetAllData() );
            ScratchImage = TexFileParser.Parse( ms );

            var layerCount = Header.ArraySize > 1 && Header.MipLevelsCount > 1 && Header.Type == Attribute.TextureType2DArray ? Header.ArraySize : Header.Depth;
            ScratchImage.GetRGBA( out var rgba );

            Layers = rgba.Images.ToArray().Select(
                image => image.Span[..( image.Width * image.Height * 4 )].ToArray() ).ToList();

            ValidFormat = ImageData.Length > 0;
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

        // ==================

        public byte[] GetAllData() => AllData;

        public byte[] GetDdsData() => DdsData;

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
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".png", "ExportedTexture", "png", ( ok, res ) => {
                if( !ok ) return;
                SaveAsPng( res );
            } );
        }

        public void SaveDdsDialog() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".dds", "ExportedTexture", "dds", ( ok, res ) => {
                if( !ok ) return;
                SaveAsDds( res );
            } );
        }

        public void SaveTexDialog( string ext ) {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", $".{ext}", "ExportedTexture", ext, ( ok, res ) => {
                if( !ok ) return;
                File.WriteAllBytes( res, GetAllData() );
            } );
        }
    }
}
