using VfxEditor.FileManager;
using VfxEditor.Select.Uld.Common;

namespace VfxEditor.Select.Uld {
    public class UldSelectDialog : SelectDialog {
        public UldSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "uld", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new CommonTab( this, "Common" ),
            } );
        }
    }
}
