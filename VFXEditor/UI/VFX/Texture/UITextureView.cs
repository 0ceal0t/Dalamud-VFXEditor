using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX {
    public class UITextureView : UINodeSplitView<UITexture> {
        public Plugin _plugin;
        public TextureManager Manager;

        public UITextureView( AVFXBase avfx, Plugin plugin ) : base( avfx, "##TEX" ) {
            _plugin = plugin;
            Manager = plugin.Manager.TexManager;
            // ==========
            List<UITexture> items = AVFX.Textures.Select( item => new UITexture( item, this ) ).ToList();
            UINode._Textures = new UINodeGroup<UITexture>( items );
            Group = UINode._Textures;
        }

        public bool GetPreviewTexture() {
            return _plugin.Configuration.PreviewTextures;
        }

        public override void OnDelete( UITexture item ) {
            AVFX.removeTexture( item.Texture );
        }

        public override UITexture OnNew() {
            return new UITexture( AVFX.addTexture(),this );
        }
    }
}
