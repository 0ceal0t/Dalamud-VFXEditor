using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Character {
    public class CharacterRowSelected {
        public readonly List<PoseData> Poses;
        public readonly GeneralData General;

        public CharacterRowSelected( List<PoseData> poses, GeneralData general ) {
            Poses = poses;
            General = general;
        }
    }
}
