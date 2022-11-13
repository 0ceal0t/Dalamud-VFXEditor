using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiTextureView : UiNodeSplitView<AvfxTexture> {
        public UiTextureView( AvfxFile file, UiNodeGroup<AvfxTexture> group ) : base( file, group, "Texture", true, true, "default_texture.vfxedit2" ) { }

        public override void OnSelect( AvfxTexture item ) { }
    }
}