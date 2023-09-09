using VfxEditor.FileManager;

namespace VfxEditor.Select.Atch {
    public class AtchSelectDialog : SelectDialog {
        public AtchSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "atch", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{

            } );
        }
    }
}
