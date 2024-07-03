using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using FFXIVClientStructs.Havok.Common.Base.Types;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Structs.Animation {
    // https://github.com/aers/FFXIVClientStructs/blob/edc749986000d056169791916fa5462a3dff3d53/FFXIVClientStructs/Havok/Animation/Mapper/hkaSkeletonMapperData.cs#L5
    public enum MappingType {
        Ragdoll = 0x0,
        Retargeting = 0x1,
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct SimpleMapping {
        public short BoneA;
        public short BoneB;
        public int Unk1;
        public int Unk2;
        public int Unk3;
        public hkQsTransformf AFromBTransform;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct PartitionMappingRange {
        public int StartMappingIndex;
        public int NumMappings;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct ChainMapping {
        public short StartBoneA;
        public short EndBoneA;
        public short StartBoneB;
        public short EndBoneB;
        public hkQsTransformf StartAFromBTransform;
        public hkQsTransformf EndAFromBTransform;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 16 )]
    public unsafe partial struct SkeletonMapperData {
        public hkRefPtr<hkaSkeleton> SkeletonA;
        public hkRefPtr<hkaSkeleton> SkeletonB;
        public hkArray<short> PartitionMap;
        public hkArray<PartitionMappingRange> SimpleMappingPartitionRanges;
        public hkArray<PartitionMappingRange> ChainMappingPartitionRanges;
        public hkArray<SimpleMapping> SimpleMappings;
        public hkArray<ChainMapping> ChainMappings;
        public hkArray<short> UnmappedBones;
        public hkQsTransformf ExtractedMotionMapping;
        public byte KeepUnmappedLocal;
        public hkEnum<MappingType, int> Type;
    }
}
