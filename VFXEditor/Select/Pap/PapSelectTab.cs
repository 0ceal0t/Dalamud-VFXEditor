using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Select.Sheets;

namespace VFXSelect.VFX {
    public abstract class PapSelectTab<T, S> : SelectTab<T, S> {
        protected readonly PapSelectDialog Dialog;

        public PapSelectTab( string parentId, string tabId, SheetLoader<T,S> loader, PapSelectDialog dialog ) : base(parentId, tabId, loader) {
            Dialog = dialog;
        }
    }
}