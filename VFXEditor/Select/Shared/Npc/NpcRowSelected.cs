using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Shared.Npc {
    public class NpcRowSelected {
        public readonly List<string> Paths;

        public NpcRowSelected( List<string> paths ) {
            Paths = paths;
        }
    }
}
