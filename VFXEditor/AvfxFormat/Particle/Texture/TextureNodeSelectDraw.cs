using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat.Particle.Texture {
    public class TextureNodeSelectDraw : IUiItem {
        private readonly List<UiNodeSelect> NodeSelects;

        public TextureNodeSelectDraw( List<UiNodeSelect> nodeSelects ) {
            NodeSelects = nodeSelects;
        }

        public void Draw() {
            foreach( var node in NodeSelects ) {
                if( node is UiNodeSelect<AvfxTexture> select ) {
                    select.Selected?.GetPreviewTexture()?.Draw();
                }
                else if( node is UiNodeSelectList<AvfxTexture> listSelect ) {
                    listSelect.Selected.ForEach( x => x?.GetPreviewTexture()?.Draw() );
                }
            }
        }
    }
}
