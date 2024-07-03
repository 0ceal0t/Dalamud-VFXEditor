using FFXIVClientStructs.Havok.Animation.Animation;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok.Structs {
    // https://github.com/Bewolf2/projectanarchy/blob/bec6e7270c9dc797a12d167d3e8942866065364e/Source/Animation/Animation/Animation/Interleaved/hkaInterleavedUncompressedAnimation.h#L18

    [StructLayout( LayoutKind.Sequential )]
    public struct HkaInterleavedUncompressedAnimation {
        public hkaAnimation Animation;

        public hkArray<hkQsTransformf> Transforms;
        public hkArray<float> Floats;
    }
}
