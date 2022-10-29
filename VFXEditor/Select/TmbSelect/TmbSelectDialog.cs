using System;
using System.Collections.Generic;

namespace VfxEditor.Select.TmbSelect {
    public class TmbSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public TmbSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "tmb", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new TmbActionSelect( id, "Action", this ),
                new TmbActionSelect( id, "Non-Player Action", this, nonPlayer:true ),
                new TmbEmoteSelect( id, "Emote", this ),
                new TmbNpcSelect( id, "Npc", this ),
                new TmbCommonSelect( id, "Common", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
