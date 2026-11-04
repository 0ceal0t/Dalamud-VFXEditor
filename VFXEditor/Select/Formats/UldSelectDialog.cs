using VfxEditor.Formats.UldFormat;
using VfxEditor.Select.Tabs.Common;

namespace VfxEditor.Select.Formats {
    public class UldSelectDialog : SelectDialog {
        public UldSelectDialog( string id, UldManager manager, bool isSourceDialog ) : base( id, "uld", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new CommonTabUld( this, "Common" ),
            ] );
        }
    }
}