using FFXIVClientStructs.Havok.Common.Base.Object;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Structs.Animation {
    [StructLayout( LayoutKind.Sequential )]
    public unsafe struct SkeletonMapper {
        public hkReferencedObject hkReferencedObject;
        public SkeletonMapperData Mapping;
    }
}