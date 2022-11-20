using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.ScdSelect {
    public class ScdSelectDialog : SelectDialog {
        public ScdSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "scd", recentList, showLocal, onSelect ) {
        }

        protected override List<SelectTab> GetTabs() => new();
    }
}
