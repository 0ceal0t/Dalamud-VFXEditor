using System.Collections.Generic;
using VfxEditor.Formats.ShpkFormat;
using VfxEditor.Select.Tabs.Common;

namespace VfxEditor.Select.Formats {
    public class ShpkSelectDialog : SelectDialog {
        public ShpkSelectDialog( string id, ShpkManager manager, bool isSourceDialog ) : base( id, "shpk", manager, isSourceDialog ) {
            GameTabs.AddRange( [
                new CommonTabShader( this, "Common", "Shpk-Common", SelectDataUtils.CommonShpkPath, ".shpk" )
            ] );
        }
    }
}