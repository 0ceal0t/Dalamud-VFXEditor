using System.Collections.Generic;

namespace VfxEditor.SklbFormat.Bones {
    public class SklbBonesImportCommand : ICommand {
        public readonly SklbBones Bones;
        public readonly List<SklbBone> State;
        public readonly List<SklbBone> PrevState;

        public SklbBonesImportCommand( SklbBones bones, List<SklbBone> state ) {
            Bones = bones;
            State = state;
            PrevState = [.. Bones.Bones];

            SetState( State );
        }

        public void Redo() => SetState( State );

        public void Undo() => SetState( PrevState );

        public void SetState( List<SklbBone> state ) {
            Bones.ClearSelected();
            Bones.Bones.Clear();
            Bones.Bones.AddRange( state );
            Bones.Updated();
        }
    }
}
