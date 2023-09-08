using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class GenericItem : INamedUiItem {
        public abstract string GetDefaultText();

        public virtual string GetText() => GetDefaultText();

        public abstract void Draw();
    }
}
