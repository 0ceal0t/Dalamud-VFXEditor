using System.Collections.Generic;

namespace VfxEditor.SklbFormat.Bones {
    public class SklbBonesImportCommand : ICommand {
        public readonly SklbBones Bones;
        public readonly List<SklbBone> OldBones = new();
        public readonly List<SklbBone> NewBones;

        public SklbBonesImportCommand( SklbBones bones, List<SklbBone> newBones ) {
            Bones = bones;
            NewBones = newBones;
        }

        public void Execute() {
            OldBones.AddRange( Bones.Bones );
            Set( NewBones );
        }

        public void Redo() => Set( NewBones );

        public void Undo() => Set( OldBones );

        public void Set( List<SklbBone> bones ) {
            Bones.ClearSelected();
            Bones.Bones.Clear();
            Bones.Bones.AddRange( bones );
            Bones.Updated();
        }
    }
}
