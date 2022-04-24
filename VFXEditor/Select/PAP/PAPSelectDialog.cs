using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiFileDialog;
using ImGuiNET;

namespace VFXSelect.PAP {
    public class PAPSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PAPSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "pap", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new PAPActionSelect( id, "Action", this ),
                new PAPEmoteSelect( id, "Emote", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
