using System.Collections.Generic;
using VfxEditor.Formats.ShcdFormat;
using VfxEditor.Select.Shared.Shader;

namespace VfxEditor.Select.Shcd {
    public class ShcdSelectDialog : SelectDialog {
        public ShcdSelectDialog( string id, ShcdManager manager, bool isSourceDialog ) : base( id, "shcd", manager, isSourceDialog ) {
            GameTabs.AddRange( new List<SelectTab>() {
                new ShaderCommonTab( this, "Shcd-Common", SelectDataUtils.CommonShcdPath, ".shcd" )
            } );
        }
    }
}
