using System;
using System.Collections.Generic;

namespace VFXEditor.Select.PapSelect {
    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "pap", recentList, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new PapActionSelect( id, "Action", this ),
                new PapActionSelect( id, "Non-Player Action", this, nonPlayer:true ),
                new PapEmoteSelect( id, "Emote", this ),
                new PapNpcSelect( id, "Npc", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
