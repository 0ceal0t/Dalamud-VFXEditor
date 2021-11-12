using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Select.Sheets;

namespace VFXSelect.VFX {
    public abstract class TmbSelectTab<T, S> : SelectTab<T, S> {
        protected readonly TmbSelectDialog Dialog;

        public TmbSelectTab( string parentId, string tabId, SheetLoader<T,S> loader, TmbSelectDialog dialog ) : base(parentId, tabId, loader) {
            Dialog = dialog;
        }
    }
}