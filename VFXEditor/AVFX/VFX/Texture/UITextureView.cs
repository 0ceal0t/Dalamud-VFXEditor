using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Texture;

namespace VfxEditor.AVFX.VFX {
    public class UITextureView : UINodeSplitView<UITexture> {
        public UITextureView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<UITexture> group ) : base( vfxFile, avfx, group, "Texture", true, true, "default_texture.vfxedit2" ) { }

        public override void OnDelete( UITexture item ) => Avfx.RemoveTexture( item.Texture );

        public override UITexture OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var tex = new AVFXTexture();
            tex.Read( reader, size );
            Avfx.AddTexture( tex );
            return new UITexture( tex );
        }

        public override void OnSelect( UITexture item ) { }
    }
}
