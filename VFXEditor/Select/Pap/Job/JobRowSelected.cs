using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Job {
    public class JobRowSelected {
        // Race -> Idle, MoveA, MoveB
        public Dictionary<string, Dictionary<string, string>> General;
        // Race -> Start, Loop
        public Dictionary<string, Dictionary<string, string>> Poses;
        // Race -> Auto1, Auto2, ...
        public Dictionary<string, Dictionary<string, string>> AutoAttack;
    }
}
