using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiFileDialog;
using ImGuiNET;

namespace VFXSelect.VFX {
    public class TmbSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public TmbSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "tmb", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new TmbActionSelect( id, "Action", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
