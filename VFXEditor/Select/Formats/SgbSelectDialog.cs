using VfxEditor.Formats.SgbFormat;

namespace VfxEditor.Select.Formats {
    public class SgbSelectDialog : SelectDialog {
        public SgbSelectDialog( string id, SgbManager manager, bool isSourceDialog ) : base( id, "sgb", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{

            } );
        }
    }
}
