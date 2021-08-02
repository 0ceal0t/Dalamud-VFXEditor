using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFXSelect.Data.Rows {
    public class XivNpcSelected {
        public XivNpc Npc;
        public List<string> VfxPaths;

        public XivNpcSelected( XivNpc npc, List<string> paths ) {
            Npc = npc;
            VfxPaths = paths;
        }
    }
}
