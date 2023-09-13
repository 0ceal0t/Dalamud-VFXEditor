using System.Runtime.InteropServices;

namespace VfxEditor.Formats.TextureFormat.CustomTeximpNet {
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    internal struct CustomPixelFormat {
        public uint Size;
        public CustomPixelFormatFlags Flags;
        public CustomFourCC CustomFourCC;
        public uint RGBBitCount;
        public uint RedBitMask;
        public uint GreenBitMask;
        public uint BlueBitMask;
        public uint AlphaBitMask;

        public bool IsDX10Extended {
            get {
                return CustomFourCC == new CustomFourCC( 'D', 'X', '1', '0' );
            }
        }

        public CustomPixelFormat( CustomPixelFormatFlags flags, CustomFourCC formatCode, uint colorBitCount, uint redMask, uint greenMask, uint blueMask, uint alphaMask ) {
            Size = ( uint )TeximpNet.MemoryHelper.SizeOf<CustomPixelFormat>();
            Flags = flags;
            CustomFourCC = formatCode;
            RGBBitCount = colorBitCount;
            RedBitMask = redMask;
            GreenBitMask = greenMask;
            BlueBitMask = blueMask;
            AlphaBitMask = alphaMask;
        }

        public CustomPixelFormat( CustomPixelFormatFlags flags, CustomFourCC formatCode ) {
            Size = ( uint )TeximpNet.MemoryHelper.SizeOf<CustomPixelFormat>();
            Flags = flags;
            CustomFourCC = formatCode;
            RGBBitCount = 0;
            RedBitMask = 0;
            GreenBitMask = 0;
            BlueBitMask = 0;
            AlphaBitMask = 0;
        }

        public override string ToString() {
            if( Flags == CustomPixelFormatFlags.FourCC ) {
                //D3D Format enum is sometimes the first char, if so then it's not a proper CustomFourCC and will just appear as a single character. Lets display the integer value instead in this case.
                var CustomFourCCValue = CustomFourCC.First > 0 && CustomFourCC.Second == 0 && CustomFourCC.Third == 0 && CustomFourCC.Fourth == 0 ? ( ( int )CustomFourCC.First ).ToString() : CustomFourCC.ToString();

                return string.Format( "Flags = {0}, CustomFourCC = '{1}'", Flags.ToString(), CustomFourCCValue );
            }

            return string.Format( "Flags = {0}, BitsPerPixel = {1}, RedMask = 0x{2}, GreenMask = 0x{3}, BlueMask = 0x{4}, AlphaMask = 0x{5}", Flags.ToString(), RGBBitCount.ToString(),
                RedBitMask.ToString( "X8" ), GreenBitMask.ToString( "X8" ), BlueBitMask.ToString( "X8" ), AlphaBitMask.ToString( "X8" ) );
        }

        //Not a format, but signifies if there exists an extended header with DXGI format enum
        public static readonly CustomPixelFormat DX10Extended = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'D', 'X', '1', '0' ) );

        //Most of these we'll write out to maximize compatability with old tools (unless noted). According to MSDN, old D3D format names should be read right to left (least to most significant bits).

        public static readonly CustomPixelFormat DXT1 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'D', 'X', 'T', '1' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat DXT2 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'D', 'X', 'T', '2' ), 0, 0, 0, 0, 0 ); //Rarely used, maps to DXT3

        public static readonly CustomPixelFormat DXT3 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'D', 'X', 'T', '3' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat DXT4 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'D', 'X', 'T', '4' ), 0, 0, 0, 0, 0 ); //Rarely used, maps to DXT5

        public static readonly CustomPixelFormat DXT5 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'D', 'X', 'T', '5' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat BC4_UNorm = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'B', 'C', '4', 'U' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat BC4_SNorm = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'B', 'C', '4', 'S' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat BC5_UNorm = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'B', 'C', '5', 'U' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat BC5_SNorm = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'B', 'C', '5', 'S' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat R8G8_B8G8 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'R', 'G', 'B', 'G' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat G8R8_G8B8 = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 'G', 'R', 'G', 'B' ), 0, 0, 0, 0, 0 );

        public static readonly CustomPixelFormat A8R8G8B8 = new( CustomPixelFormatFlags.RGBA, 0, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000 );

        public static readonly CustomPixelFormat X8R8G8B8 = new( CustomPixelFormatFlags.RGB, 0, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000 );

        public static readonly CustomPixelFormat A8B8G8R8 = new( CustomPixelFormatFlags.RGBA, 0, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000 );

        public static readonly CustomPixelFormat X8B8G8R8 = new( CustomPixelFormatFlags.RGB, 0, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0x00000000 ); //Use DX10, Maps to DXGI R8G8B8A8_UNorm

        public static readonly CustomPixelFormat G16R16 = new( CustomPixelFormatFlags.RGB, 0, 32, 0x0000ffff, 0xffff0000, 0x00000000, 0x00000000 );

        public static readonly CustomPixelFormat R5G6B5 = new( CustomPixelFormatFlags.RGB, 0, 16, 0x0000f800, 0x000007e0, 0x0000001f, 0x00000000 );

        public static readonly CustomPixelFormat A1R5G5B5 = new( CustomPixelFormatFlags.RGBA, 0, 16, 0x00007c00, 0x000003e0, 0x0000001f, 0x00008000 );

        public static readonly CustomPixelFormat A4R4G4B4 = new( CustomPixelFormatFlags.RGBA, 0, 16, 0x00000f00, 0x000000f0, 0x0000000f, 0x0000f000 );

        public static readonly CustomPixelFormat R8G8B8 = new( CustomPixelFormatFlags.RGB, 0, 24, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000 ); //Use DX10, Maps to R8G8B8A8_UNorm with opaque alpha

        public static readonly CustomPixelFormat L8 = new( CustomPixelFormatFlags.Luminance, 0, 8, 0xff, 0x00, 0x00, 0x00 );

        public static readonly CustomPixelFormat L16 = new( CustomPixelFormatFlags.Luminance, 0, 16, 0xffff, 0x0000, 0x0000, 0x0000 );

        public static readonly CustomPixelFormat A8L8 = new( CustomPixelFormatFlags.LuminanceAlpha, 0, 16, 0x00ff, 0x0000, 0x0000, 0xff00 );

        public static readonly CustomPixelFormat A8 = new( CustomPixelFormatFlags.Alpha, 0, 8, 0x00, 0x00, 0x00, 0xff );


        //Some common legacy D3DX formats that use D3DFMT enum value as CustomFourCC. The names correspond to the DXGI format name.

        public static readonly CustomPixelFormat R32G32B32A32_Float = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 116 ) ); // D3DFMT_A32B32G32R32F

        public static readonly CustomPixelFormat R16G16B16A16_Float = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 113 ) ); // D3DFMT_A16B16G16R16F

        public static readonly CustomPixelFormat R16G16B16A16_UNorm = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 36 ) ); // D3DFMT_A16B16G16R16

        public static readonly CustomPixelFormat R16G16B16A16_SNorm = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 110 ) ); // D3DFMT_Q16W16V16U16

        public static readonly CustomPixelFormat R32G32_Float = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 115 ) ); // D3DFMT_G32R32F

        public static readonly CustomPixelFormat R16G16_Float = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 112 ) ); // D3DFMT_G16R16F

        public static readonly CustomPixelFormat R32_Float = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 114 ) ); // D3DFMT_R32F

        public static readonly CustomPixelFormat R16_Float = new( CustomPixelFormatFlags.FourCC, new CustomFourCC( 111 ) ); // D3DFMT_R16F
    }
}
