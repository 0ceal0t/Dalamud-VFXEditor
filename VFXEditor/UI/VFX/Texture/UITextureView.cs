using AVFXLib.Models;
using System.Linq;
using AVFXLib.AVFX;
using VFXEditor.Data.Texture;

namespace VFXEditor.UI.VFX {
    public class UITextureView : UINodeSplitView<UITexture> {

        public UITextureView( UIMain main, AVFXBase avfx) : base( main, avfx, "##TEX" ) {
            Group = main.Textures;
            Group.Items = AVFX.Textures.Select( item => new UITexture( Main, item ) ).ToList();
        }

        public override void OnDelete( UITexture item ) {
            AVFX.RemoveTexture( item.Texture );
        }

        public override UITexture OnImport( AVFXNode node, bool has_dependencies = false ) {
            var tex = new AVFXTexture();
            tex.Read( node );
            AVFX.AddTexture( tex );
            return new UITexture( Main, tex );
        }

        public override UITexture OnNew() {
            return new UITexture( Main, AVFX.AddTexture() );
        }
    }
}
