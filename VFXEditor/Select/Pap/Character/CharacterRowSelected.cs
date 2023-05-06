using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Character {
    public class CharacterRowSelected {
        // Idle, MoveA, MoveB
        public readonly Dictionary<string, string> General;
        // Pose # -> Start, Loop
        public readonly Dictionary<string, Dictionary<string, string>> Poses;

        public CharacterRowSelected( Dictionary<string, string> general, Dictionary<string, Dictionary<string, string>> poses ) {
            Poses = poses;
            General = general;
        }
    }
}
