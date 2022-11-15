namespace VfxEditor.AvfxFormat2 {
    public interface IUiSelectableItem : IUiItem {
        public int GetIdx();
        public void SetIdx(int idx);
    }
}