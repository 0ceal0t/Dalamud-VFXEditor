using System.IO;
using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat {
    public class UiTextureView : UiNodeSplitView<AvfxTexture> {
        public UiTextureView( AvfxFile file, NodeGroup<AvfxTexture> group ) : base( "Textures", file, group, "Texture", true, true, "default_texture.vfxedit2" ) { }

        public override void OnSelect( AvfxTexture item ) { }

        public override AvfxTexture Read( BinaryReader reader, int size ) {
            var item = new AvfxTexture();
            item.Read( reader, size );
            return item;
        }
    }
}