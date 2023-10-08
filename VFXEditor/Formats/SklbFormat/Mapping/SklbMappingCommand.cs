using FFXIVClientStructs.Havok;
using VfxEditor.Interop.Structs.Animation;

namespace VfxEditor.Formats.SklbFormat.Mapping {
    public unsafe class SklbMappingCommand : ICommand {
        private readonly SkeletonMapper* Mapper;
        private readonly hkaSkeleton* NewSkeleton;
        private hkaSkeleton* OldSkeleton;

        public SklbMappingCommand( SkeletonMapper* mapper, hkaSkeleton* newSkeleton ) {
            Mapper = mapper;
            NewSkeleton = newSkeleton;
        }

        public void Execute() {
            OldSkeleton = Mapper->Mapping.SkeletonB.ptr;
            Mapper->Mapping.SkeletonB.ptr = NewSkeleton;
        }

        public void Redo() {
            Mapper->Mapping.SkeletonB.ptr = NewSkeleton;
        }

        public void Undo() {
            Mapper->Mapping.SkeletonB.ptr = OldSkeleton;
        }
    }
}
