using System.Runtime.InteropServices;
using TeximpNet.DDS;

namespace VfxEditor.Formats.TextureFormat.CustomTeximpNet {
    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    internal struct CustomHeader10 {
        public DXGIFormat Format;
        public CustomD3D10ResourceDimension ResourceDimension;
        public CustomHeader10Flags MiscFlags;
        public uint ArraySize;
        public CustomHeader10Flags2 MiscFlags2;

        public CustomHeader10( DXGIFormat format, CustomD3D10ResourceDimension resourceDim, CustomHeader10Flags miscFlags, uint arraySize, CustomHeader10Flags2 miscFlags2 ) {
            Format = format;
            ResourceDimension = resourceDim;
            MiscFlags = miscFlags;
            ArraySize = arraySize;
            MiscFlags2 = miscFlags2;
        }
    }
}
