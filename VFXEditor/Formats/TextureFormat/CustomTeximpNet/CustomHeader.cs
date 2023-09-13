using System.Runtime.InteropServices;

namespace VfxEditor.Formats.TextureFormat.CustomTeximpNet {
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    internal unsafe struct CustomHeader {
        public uint Size;
        public CustomHeaderFlags Flags;
        public uint Height;
        public uint Width;
        public uint PitchOrLinearSize;
        public uint Depth;
        public uint MipMapCount;
        public fixed uint Reserved1[11];
        public CustomPixelFormat PixelFormat;
        public CustomHeaderCaps Caps;
        public CustomHeaderCaps2 Caps2;
        public uint Caps3;
        public uint Caps4;
        public uint Reserved2;
    }
}
