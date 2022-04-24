using System;
using System.Collections.Generic;

namespace VFXSelect.TMB {
    public class TMBSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public TMBSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "tmb", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new TMBActionSelect( id, "Action", this ),
                new TMBEmoteSelect( id, "Emote", this)
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
