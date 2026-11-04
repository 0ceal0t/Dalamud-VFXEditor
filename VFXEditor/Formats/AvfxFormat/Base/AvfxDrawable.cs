using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AvfxFormat.Base {
    public abstract class AvfxDrawable : AvfxBase, IUiItem {
        public AvfxDrawable( string avfxName ) : base( avfxName ) { }

        public abstract void Draw();
    }
}