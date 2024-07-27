using VfxEditor.Formats.PbdFormat;
using VfxEditor.Select.Tabs.Common;

namespace VfxEditor.Select.Formats {
    public class PbdSelectDialog : SelectDialog {
        public PbdSelectDialog( string id, PbdManager manager, bool isSourceDialog ) : base( id, "pbd", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new CommonTabPbd( this, "Common" ),
            ] );
        }
    }
}
