using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXSelect.Select.Sheets;

namespace VFXSelect.TMB {
    public abstract class TMBSelectTab<T, S> : SelectTab<T, S> {
        protected readonly TMBSelectDialog Dialog;

        public TMBSelectTab( string parentId, string tabId, SheetLoader<T,S> loader, TMBSelectDialog dialog ) : base(parentId, tabId, loader) {
            Dialog = dialog;
        }
    }
}