using VfxEditor.Select.Tabs.Common;
using VfxEditor.UldFormat;

namespace VfxEditor.Select.Formats {
    public class UldSelectDialog : SelectDialog {
        public UldSelectDialog( string id, UldManager manager, bool isSourceDialog ) : base( id, "uld", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new CommonTabUld( this, "Common" ),
            } );
        }
    }
}