using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select2.Pap.Action;
using VfxEditor.Select2.Pap.Emote;
using VfxEditor.Select2.Pap.Npc;

namespace VfxEditor.Select2.Pap {
    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "pap", manager, isSourceDialog ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ActionTab( this, "Action" ),
                new NonPlayerActionTab( this, "Non-Player Action" ),
                new EmoteTab( this, "Emote" ),
                new NpcPapTab( this, "Npc" )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
