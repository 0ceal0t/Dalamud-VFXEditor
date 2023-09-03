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
            OldAnimations = File.MotionData.AnimationContainer->Animations;
            OldBindings = File.MotionData.AnimationContainer->Bindings;
        }

        public void Execute() {
            ChangeAction.Invoke();
            NewAnimations = File.MotionData.AnimationContainer->Animations;
            NewBindings = File.MotionData.AnimationContainer->Bindings;
            File.MotionData.UpdateMotions();
        }

        public void Undo() {
            File.MotionData.AnimationContainer->Animations = OldAnimations;
            File.MotionData.AnimationContainer->Bindings = OldBindings;
            File.MotionData.UpdateMotions();
        }

        public void Redo() {
            File.MotionData.AnimationContainer->Animations = NewAnimations;
            File.MotionData.AnimationContainer->Bindings = NewBindings;
            File.MotionData.UpdateMotions();
        }
    }
}
