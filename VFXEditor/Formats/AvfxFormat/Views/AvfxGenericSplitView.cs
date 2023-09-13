using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxGenericSplitView<T> : ItemSplitView<T>, IUiItem where T : class {
        protected bool AllowDelete;

        public AvfxGenericSplitView( string id, List<T> items, bool allowNew, bool allowDelete ) : base( id, items, allowNew ) {
            AllowDelete = allowDelete;
        }
    }
}
