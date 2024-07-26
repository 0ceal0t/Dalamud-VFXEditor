using VfxEditor.Formats.PbdFormat;

namespace VfxEditor.Select.Formats {
    public class PbdSelectDialog : SelectDialog {
        public PbdSelectDialog( string id, PbdManager manager, bool isSourceDialog ) : base( id, "pbd", manager, isSourceDialog ) {
            GameTabs.AddRange( [] );
        }
    }
}
