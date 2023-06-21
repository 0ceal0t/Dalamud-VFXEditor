using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Select.Phyb {
    public class PhybSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PhybSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "phyb", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
