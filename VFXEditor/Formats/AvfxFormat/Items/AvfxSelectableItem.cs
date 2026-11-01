using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.AvfxFormat.Items {
    public abstract class AvfxSelectableItem : AvfxItem, IIndexUiItem {
        private int Idx;

        public AvfxSelectableItem( string avfxName ) : base( avfxName ) { }

        public int GetIdx() => Idx;

        public void SetIdx( int idx ) { Idx = idx; }
    }
}
