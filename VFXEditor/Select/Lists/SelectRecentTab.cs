using System;
using System.Collections.Generic;

namespace VfxEditor.Select.Lists {
    public class SelectRecentTab : SelectListTab<SelectResult> {
        public SelectRecentTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override SelectResult ItemToSelectResult( SelectResult item ) => item;
    }
}
