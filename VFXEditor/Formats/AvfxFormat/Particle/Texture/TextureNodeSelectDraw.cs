using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat.Particle.Texture {
    public class TextureNodeSelectDraw : IUiItem {
        private readonly List<AvfxNodeSelect> NodeSelects;

        public TextureNodeSelectDraw( List<AvfxNodeSelect> nodeSelects ) {
            NodeSelects = nodeSelects;
        }

        public void Draw() {
            foreach( var node in NodeSelects ) {
                if( node is AvfxNodeSelect<AvfxTexture> select ) {
                    select.Selected?.GetTexture()?.DrawImage();
                }
                else if( node is AvfxNodeSelectList<AvfxTexture> listSelect ) {
                    listSelect.Selected.ForEach( x => x?.GetTexture()?.DrawImage() );
                }
            }
        }
    }
}
