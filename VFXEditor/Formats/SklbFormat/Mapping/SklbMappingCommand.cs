using FFXIVClientStructs.Havok;
using VfxEditor.Interop.Structs.Animation;

namespace VfxEditor.Formats.SklbFormat.Mapping {
    public unsafe class SklbMappingCommand : ICommand {
        private readonly SkeletonMapper* Mapper;
        private readonly hkaSkeleton* State;
        private readonly hkaSkeleton* PrevState;

        public SklbMappingCommand( SkeletonMapper* mapper, hkaSkeleton* state ) {
            Mapper = mapper;
            State = state;
            PrevState = Mapper->Mapping.SkeletonA.ptr;

            Mapper->Mapping.SkeletonA.ptr = State;
        }

        public void Redo() {
            Mapper->Mapping.SkeletonA.ptr = State;
        }

        public void Undo() {
            Mapper->Mapping.SkeletonA.ptr = PrevState;
        }
    }
}
