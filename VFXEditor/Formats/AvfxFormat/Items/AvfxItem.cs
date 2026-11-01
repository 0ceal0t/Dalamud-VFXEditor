using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AvfxFormat.Items {
    public abstract class AvfxItem : AvfxDrawable, INamedUiItem {
        public AvfxItem( string avfxName ) : base( avfxName ) { }

        public abstract string GetDefaultText();

        public virtual string GetText() => GetDefaultText();
    }
}
