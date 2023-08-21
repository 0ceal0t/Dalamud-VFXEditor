using FFXIVClientStructs.Havok;
using System;

namespace VfxEditor.PapFormat {
    public unsafe class PapHavokCommand : ICommand {
        private readonly PapFile File;
        private readonly Action ChangeAction;
        private readonly hkArray<hkRefPtr<hkaAnimation>> OldAnimations;
        private readonly hkArray<hkRefPtr<hkaAnimationBinding>> OldBindings;
        private hkArray<hkRefPtr<hkaAnimation>> NewAnimations;
        private hkArray<hkRefPtr<hkaAnimationBinding>> NewBindings;

        public PapHavokCommand( PapFile file, Action changeAction ) {
            File = file;
            ChangeAction = changeAction;

            // Back up data
            OldAnimations = File.AnimationData.AnimationContainer->Animations;
            OldBindings = File.AnimationData.AnimationContainer->Bindings;
        }

        public void Execute() {
            ChangeAction.Invoke();
            NewAnimations = File.AnimationData.AnimationContainer->Animations;
            NewBindings = File.AnimationData.AnimationContainer->Bindings;
            File.AnimationData.UpdateAnimations();
        }

        public void Undo() {
            File.AnimationData.AnimationContainer->Animations = OldAnimations;
            File.AnimationData.AnimationContainer->Bindings = OldBindings;
            File.AnimationData.UpdateAnimations();
        }

        public void Redo() {
            File.AnimationData.AnimationContainer->Animations = NewAnimations;
            File.AnimationData.AnimationContainer->Bindings = NewBindings;
            File.AnimationData.UpdateAnimations();
        }
    }
}
