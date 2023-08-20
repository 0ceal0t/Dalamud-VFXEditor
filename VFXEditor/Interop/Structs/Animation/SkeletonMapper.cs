using FFXIVClientStructs.Havok;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Structs.Animation {
    [StructLayout( LayoutKind.Sequential )]
    public struct SkeletonMapper {
        public hkReferencedObject hkReferencedObject;
        public SkeletonMapperData Mapping;
    }
}