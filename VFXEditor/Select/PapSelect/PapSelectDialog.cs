using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Select.PapSelect {
    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "pap", manager, isSourceDialog ) {

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
