namespace VfxEditor.Ui.Interfaces {
    public interface ISelectableUiItem : INamedUiItem {
        public int GetIdx();

        public void SetIdx( int idx );
    }
}
