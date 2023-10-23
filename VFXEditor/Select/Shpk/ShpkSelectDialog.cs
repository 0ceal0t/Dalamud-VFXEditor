using System.Collections.Generic;
using VfxEditor.Formats.ShpkFormat;
using VfxEditor.Select.Shared.Shader;

namespace VfxEditor.Select.Shpk {
    public class ShpkSelectDialog : SelectDialog {
        public ShpkSelectDialog( string id, ShpkManager manager, bool isSourceDialog ) : base( id, "shpk", manager, isSourceDialog ) {
            GameTabs.AddRange( new List<SelectTab>() {
                new ShaderCommonTab( this, "Shpk-Common", SelectDataUtils.CommonShpkPath, ".shpk" )
            } );
        }
    }
}
