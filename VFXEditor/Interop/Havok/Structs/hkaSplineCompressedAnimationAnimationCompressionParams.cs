using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok.Structs {
    // https://github.com/Bewolf2/projectanarchy/blob/bec6e7270c9dc797a12d167d3e8942866065364e/Source/Animation/Animation/Animation/SplineCompressed/hkaSplineCompressedAnimation.h#L90

    [StructLayout( LayoutKind.Sequential )]
    public struct hkaSplineCompressedAnimationAnimationCompressionParams {
        public ushort MaxFramesPerBlock;
        public bool EnableSampleSingleTracks;
        public byte Padding;
    }
}
