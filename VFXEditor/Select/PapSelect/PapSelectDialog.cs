using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Select.PapSelect {
    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog(
                string id,
                List<SelectResult> recentList,
                bool showLocal,
                Action<SelectResult> onSelect
            ) : base( id, "pap", recentList, Plugin.Configuration.FavoritePap, showLocal, onSelect ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new PapActionSelect( "Action", this ),
                new PapActionSelect( "Non-Player Action", this, nonPlayer:true ),
                new PapEmoteSelect( "Emote", this ),
                new PapNpcSelect( "Npc", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
