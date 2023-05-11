using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxItem : AvfxDrawable, INamedUiItem {
        public AvfxItem( string avfxName ) : base( avfxName ) { }

        public abstract string GetDefaultText();

        public virtual string GetText() => GetDefaultText();
    }
}
