using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AvfxFormat.Items {
    public abstract class GenericSelectableItem : GenericItem, IIndexUiItem {
        private int Idx;

        public int GetIdx() => Idx;

        public void SetIdx( int idx ) { Idx = idx; }
    }
}
