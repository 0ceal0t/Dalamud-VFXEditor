using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiTextureView : UiNodeSplitView<AvfxTexture> {
        public UiTextureView( AvfxFile file, UiNodeGroup<AvfxTexture> group ) : base( file, group, "Texture", true, true, "default_texture.vfxedit2" ) { }

        public override void OnSelect( AvfxTexture item ) { }

        public override AvfxTexture Read( BinaryReader reader, int size, bool hasDependencies ) {
            var item = new AvfxTexture();
            item.Read( reader, size );
            return item;
        }
    }
}