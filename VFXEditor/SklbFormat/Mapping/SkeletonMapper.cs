using FFXIVClientStructs.Havok;
using System.Runtime.InteropServices;

namespace VfxEditor.SklbFormat.Mapping {
    [StructLayout( LayoutKind.Sequential )]
    public struct SkeletonMapper {
        public hkReferencedObject hkReferencedObject;
        public SkeletonMapperData Mapping;
    }
}