using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.NpcID {
    public class NpcIDTabTmb : NpcTab {
        public NpcIDTabTmb( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = files.tmb;
        }
    }
}