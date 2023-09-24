using System;
using VfxEditor.Formats.ShpkFormat;

namespace VfxEditor.Select.Shpk {
    public class ShpkSelectDialog : SelectDialog {
        public ShpkSelectDialog( string id, ShpkManager manager, bool isSourceDialog ) : base( id, "shpk", manager, isSourceDialog ) {
            GameTabs.AddRange( Array.Empty<SelectTab>() );
        }
    }
}
