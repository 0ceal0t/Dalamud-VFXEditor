using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Character {
    public class CharacterRowSelected {
        // Idle, MoveA, MoveB
        public Dictionary<string, string> General;
        // Pose # -> Start, Loop
        public Dictionary<string, Dictionary<string, string>> Poses;
    }
}
