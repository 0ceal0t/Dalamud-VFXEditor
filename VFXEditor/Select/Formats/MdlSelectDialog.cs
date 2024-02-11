using VfxEditor.Formats.MdlFormat;

namespace VfxEditor.Select.Formats {
    public class MdlSelectDialog : SelectDialog {
        public MdlSelectDialog( string id, MdlManager manager, bool isSourceDialog ) : base( id, "mdl", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{

            } );
        }
    }
}
