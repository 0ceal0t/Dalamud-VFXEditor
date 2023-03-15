using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Components;

namespace VfxEditor.AvfxFormat {
    public abstract class AvfxGenericSplitView<T> : SplitView<T>, IAvfxUiBase where T : class {
        protected bool AllowDelete;

        public AvfxGenericSplitView( List<T> items, bool allowNew, bool allowDelete ) : base( items, allowNew ) {
            AllowDelete = allowDelete;
        }
    }
}
