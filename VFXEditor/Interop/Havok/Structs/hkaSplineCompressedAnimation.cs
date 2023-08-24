using FFXIVClientStructs.Havok;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok.Structs {
    // https://github.com/Bewolf2/projectanarchy/blob/bec6e7270c9dc797a12d167d3e8942866065364e/Source/Animation/Animation/Animation/SplineCompressed/hkaSplineCompressedAnimation.h#L297

    [StructLayout( LayoutKind.Sequential )]
    public struct hkaSplineCompressedAnimation {
        public hkaAnimation Animation;

        public int NumFrames;

        public int NumBlocks;
        public int MaxFramesPerBlock;
        public int MaskAndQuantizationSize;
        public float BlockDuration;
        public float BlockInverseDuration;
        public float FrameDuration;
        public uint Padding1;

        public hkArray<uint> BlockOffsets;
        public hkArray<uint> FloatBlockOffsets;
        public hkArray<uint> TransformOffsets;
        public hkArray<uint> FloatOffsets;
        public hkArray<byte> Data;
        public int Endian;
        public uint Padding2;
    }
}
