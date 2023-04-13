using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxGenericSplitView<T> : SplitView<T>, IUiItem where T : class {
        protected bool AllowDelete;

        public AvfxGenericSplitView( List<T> items, bool allowNew, bool allowDelete ) : base( items, allowNew ) {
            AllowDelete = allowDelete;
        }
    }
}
