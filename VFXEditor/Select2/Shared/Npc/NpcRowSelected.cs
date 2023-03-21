using System;
using System.Collections.Generic;

namespace VfxEditor.Select2.Shared.Npc {
    public class NpcRowSelected {
        public readonly List<string> Paths;

        public NpcRowSelected( List<string> paths ) {
            Paths = paths;
        }
    }
}
