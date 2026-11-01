using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AvfxFormat.Items {
    public abstract class GenericItem : INamedUiItem {
        public abstract string GetDefaultText();

        public virtual string GetText() => GetDefaultText();

        public abstract void Draw();
    }
}
