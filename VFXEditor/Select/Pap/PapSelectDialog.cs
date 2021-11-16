using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiFileDialog;
using ImGuiNET;

namespace VFXSelect.VFX {
    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "pap", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new PapActionSelect( id, "Action", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
