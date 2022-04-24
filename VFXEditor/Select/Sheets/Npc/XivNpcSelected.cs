using System.Collections.Generic;

namespace VFXSelect.Select.Rows {
    public class XivNpcSelected {
        public XivNpc Npc;
        public List<string> VfxPaths;

        public XivNpcSelected( XivNpc npc, List<string> paths ) {
            Npc = npc;
            VfxPaths = paths;
        }
    }
}
