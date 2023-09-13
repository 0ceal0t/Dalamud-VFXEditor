using System;

namespace VfxEditor.Formats.TextureFormat.CustomTeximpNet {
    internal enum CustomD3D10ResourceDimension : uint {
        Unknown = 0,
        Buffer = 1,
        Texture1D = 2,
        Texture2D = 3,
        Texture3D = 4
    }

    [Flags]
    internal enum CustomHeaderFlags : uint {
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
    internal enum CustomHeaderCaps : uint {
        None = 0,
        Complex = 0x8,
        Texture = 0x1000,
        MipMap = 0x400000
    }

    [Flags]
    internal enum CustomHeaderCaps2 : uint {
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
    internal enum CustomHeader10Flags : uint {
        None = 0,
        TextureCube = 0x4
    }

    [Flags]
    internal enum CustomHeader10Flags2 : uint {
        None = 0,
        AlphaModeStraight = 0x1,
        AlphaModePremultiplied = 0x2,
        AlphaModeOpaque = 0x3,
        AlphaModeCustom = 0x4
    }


    [Flags]
    internal enum CustomPixelFormatFlags : uint {
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
}
