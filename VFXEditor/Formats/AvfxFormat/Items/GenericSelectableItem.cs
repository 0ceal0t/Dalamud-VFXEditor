using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class GenericSelectableItem : GenericItem, IIndexUiItem {
        private int Idx;

        public int GetIdx() => Idx;

        public void SetIdx( int idx ) { Idx = idx; }
    }
}
