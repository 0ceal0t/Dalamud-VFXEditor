using System.Collections.Generic;
using VfxEditor.Formats.MtrlFormat;

namespace VfxEditor.Select.Formats {
    public class MtrlSelectDialog : SelectDialog {
        public MtrlSelectDialog( string id, MtrlManager manager, bool isSourceDialog ) : base( id, "mtrl", manager, isSourceDialog ) {
            GameTabs.AddRange( new List<SelectTab>() {
            } );
        }
    }
}