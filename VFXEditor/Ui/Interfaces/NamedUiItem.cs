namespace VfxEditor.Ui.Interfaces {
    public interface INamedUiItem : IUiItem {
        public string GetDefaultText();

        public string GetText();
    }
}
