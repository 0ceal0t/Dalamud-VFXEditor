namespace VfxEditor.Ui.Interfaces {
    public interface IIndexUiItem : INamedUiItem {
        public int GetIdx();

        public void SetIdx( int idx );
    }
}
