using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Texture;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTextureView : UiNodeSplitView<UiTexture> {
        public UiTextureView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiTexture> group ) : base( vfxFile, avfx, group, "Texture", true, true, "default_texture.vfxedit2" ) { }

        public override void RemoveFromAvfx( UiTexture item ) => Avfx.Textures.Remove( item.Texture );

        public override void AddToAvfx( UiTexture item, int idx ) => Avfx.Textures.Insert( idx, item.Texture );

        public override UiTexture OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var tex = new AVFXTexture();
            tex.Read( reader, size );
            Avfx.Textures.Add( tex );
            return new UiTexture( tex );
        }

        public override void OnSelect( UiTexture item ) { }
    }
}
