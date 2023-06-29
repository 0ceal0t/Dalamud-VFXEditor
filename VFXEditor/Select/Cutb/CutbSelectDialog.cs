using VfxEditor.FileManager;

namespace VfxEditor.Select.Cutb {
    public class CutbSelectDialog : SelectDialog {
        public CutbSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "cutb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
            } );
        }
    }
}
