using VfxEditor.Formats.KdbFormat;

namespace VfxEditor.Select.Formats {
    public class KdbSelectDialog : SelectDialog {
        public KdbSelectDialog( string id, KdbManager manager, bool isSourceDialog ) : base( id, "kdb", manager, isSourceDialog ) { }
    }
}
