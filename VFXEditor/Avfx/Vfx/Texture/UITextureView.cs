using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Texture;

namespace VFXEditor.Avfx.Vfx {
    public class UITextureView : UINodeSplitView<UITexture> {
        public UITextureView( AvfxFile main, AVFXMain avfx ) : base( main, avfx, "##TEX" ) {
            Group = main.Textures;
            Group.Items = AVFX.Textures.Select( item => new UITexture( Main, item ) ).ToList();
        }

        public override void OnDelete( UITexture item ) {
            AVFX.RemoveTexture( item.Texture );
        }

        public override UITexture OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var tex = new AVFXTexture();
            tex.Read( reader, size );
            AVFX.AddTexture( tex );
            return new UITexture( Main, tex );
        }

        public override UITexture OnNew() {
            return new UITexture( Main, AVFX.AddTexture() );
        }
    }
}
