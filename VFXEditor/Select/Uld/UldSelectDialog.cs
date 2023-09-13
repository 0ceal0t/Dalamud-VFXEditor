using VfxEditor.Select.Uld.Common;
using VfxEditor.UldFormat;

namespace VfxEditor.Select.Uld {
    public class UldSelectDialog : SelectDialog {
        public UldSelectDialog( string id, UldManager manager, bool isSourceDialog ) : base( id, "uld", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new CommonTab( this, "Common" ),
            } );
        }
    }
}
