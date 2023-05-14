using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxDrawable : AvfxBase, IUiItem {
        public AvfxDrawable( string avfxName ) : base( avfxName ) { }

        public abstract void Draw();
    }
}
