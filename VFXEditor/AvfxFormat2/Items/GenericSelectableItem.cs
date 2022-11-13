using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public abstract class GenericSelectableItem : GenericItem, IUiSelectableItem {
        private int Idx;

        public int GetIdx() => Idx;
        public void SetIdx( int idx ) { Idx = idx; }
    }
}
