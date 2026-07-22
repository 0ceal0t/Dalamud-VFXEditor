using System.Collections.Generic;

namespace VfxEditor.Select.Tabs.NpcID {
    public class NpcIDTabVfx : NpcTab {
        public NpcIDTabVfx( SelectDialog dialog, string name ) : base( dialog, name ) { }

        protected override void GetLoadedFiles( NpcFilesStruct files, out List<string> loaded ) {
            loaded = files.vfx;
        }
    }
}