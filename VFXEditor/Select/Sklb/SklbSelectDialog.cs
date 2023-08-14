using VfxEditor.FileManager;

namespace VfxEditor.Select.Sklb {
    public class SklbSelectDialog : SelectDialog {
        public SklbSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "sklb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{

            } );
        }
    }
}
