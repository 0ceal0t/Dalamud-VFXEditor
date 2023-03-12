namespace VfxEditor.AvfxFormat {
    public interface IUiSelectableItem : IUiItem {
        public int GetIdx();
        public void SetIdx( int idx );
    }
}