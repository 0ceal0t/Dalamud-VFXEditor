using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Select.Sheets;

namespace VFXSelect.PAP {
    public abstract class PAPSelectTab<T, S> : SelectTab<T, S> {
        protected readonly PAPSelectDialog Dialog;

        public PAPSelectTab( string parentId, string tabId, SheetLoader<T,S> loader, PAPSelectDialog dialog ) : base(parentId, tabId, loader) {
            Dialog = dialog;
        }
    }
}