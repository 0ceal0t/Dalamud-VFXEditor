using System;

namespace VfxEditor.AvfxFormat {
    public abstract class GenericSelectableItem : GenericItem, IUiSelectableItem {
        private int Idx;

        public int GetIdx() => Idx;
        public void SetIdx( int idx ) { Idx = idx; }
    }
}
