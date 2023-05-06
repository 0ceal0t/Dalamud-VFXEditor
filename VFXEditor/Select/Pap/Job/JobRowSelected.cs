using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Pap.Job {
    public class JobRowSelected {
        // Race -> Idle, MoveA, MoveB
        public readonly Dictionary<string, Dictionary<string, string>> General;
        // Race -> Start, Loop
        public readonly Dictionary<string, Dictionary<string, string>> Poses;
        // Race -> Auto1, Auto2, ...
        public readonly Dictionary<string, Dictionary<string, string>> AutoAttack;

        public JobRowSelected( Dictionary<string, Dictionary<string, string>> general, Dictionary<string, Dictionary<string, string>> poses, Dictionary<string, Dictionary<string, string>> autoAttack ) {
            General = general;
            Poses = poses;
            AutoAttack = autoAttack;
        }
    }
}
