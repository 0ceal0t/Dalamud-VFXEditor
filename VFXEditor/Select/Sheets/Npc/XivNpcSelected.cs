using System.Collections.Generic;
using static VfxEditor.Select.Sheets.NpcSheetLoader;

namespace VfxEditor.Select.Rows {
    public class XivNpcSelected {
        public readonly XivNpc Npc;
        public readonly List<string> VfxPaths;
        public readonly List<string> TmbPaths;
        public readonly List<string> PapPaths;

        public XivNpcSelected( XivNpc npc, List<string> vfxPaths, List<string> tmbPaths, List<string> papPaths ) {
            Npc = npc;
            VfxPaths = vfxPaths;
            TmbPaths = tmbPaths;
            PapPaths = papPaths;
        }
    }
}
