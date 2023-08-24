using FFXIVClientStructs.Havok;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok.Structs {
    // https://github.com/Bewolf2/projectanarchy/blob/bec6e7270c9dc797a12d167d3e8942866065364e/Source/Animation/Animation/Animation/SplineCompressed/hkaSplineCompressedAnimation.h#L38

    public enum RotationQuantization : byte {
        POLAR32 = 0,
        THREECOMP40 = 1,
        THREECOMP48 = 2,
        THREECOMP24 = 3,
        STRAIGHT16 = 4,
        UNCOMPRESSED = 5,
    }

    public enum ScalarQuantization : byte {
        BITS8 = 0,
        BITS16 = 1,
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct hkaSplineCompressedAnimationTrackCompressionParams {
        public float RotationTolerance;
        public float TranslationTolerance;
        public float ScaleTolerance;
        public float FloatingTolerance;

        public ushort RotationDegree;
        public ushort TranslationDegree;
        public ushort ScaleDegree;
        public ushort FloatingDegree;

        public hkEnum<RotationQuantization, byte> RotationQuantizationType;
        public hkEnum<ScalarQuantization, byte> TranslationQuantizationType;
        public hkEnum<ScalarQuantization, byte> ScaleQuantizationType;
        public hkEnum<ScalarQuantization, byte> FloatingQuantizationType;
    }
}
