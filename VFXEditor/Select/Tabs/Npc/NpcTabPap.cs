using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.Npc {
    public class NpcTabPap : NpcTab {
        public NpcTabPap( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = files.pap;
        }
    }
}