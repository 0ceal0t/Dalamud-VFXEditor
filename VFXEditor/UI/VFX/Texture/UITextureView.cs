using AVFXLib.Models;
using System.Linq;
using AVFXLib.AVFX;
using VFXEditor.Data.Texture;

namespace VFXEditor.UI.VFX {
    public class UITextureView : UINodeSplitView<UITexture> {
        public Plugin _plugin;
        public TextureManager Manager;

        public UITextureView( AVFXBase avfx, Plugin plugin ) : base( avfx, "##TEX" ) {
            _plugin = plugin;
            Manager = plugin.Manager.TexManager;
            // ==========
            Group = UINodeGroup.Textures;
            Group.Items = AVFX.Textures.Select( item => new UITexture( item, this ) ).ToList();
        }

        public override void OnDelete( UITexture item ) {
            AVFX.RemoveTexture( item.Texture );
        }

        public override UITexture OnImport( AVFXNode node ) {
            AVFXTexture tex = new AVFXTexture();
            tex.Read( node );
            AVFX.AddTexture( tex );
            return new UITexture( tex, this );
        }

        public override UITexture OnNew() {
            return new UITexture( AVFX.AddTexture(),this );
        }
    }
}
