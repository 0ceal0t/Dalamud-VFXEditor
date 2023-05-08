using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Pap.Action;
using VfxEditor.Select.Pap.Emote;
using VfxEditor.Select.Pap.IdlePose;
using VfxEditor.Select.Pap.Job;
using VfxEditor.Select.Pap.Mount;
using VfxEditor.Select.Pap.Npc;
using VfxEditor.Select.Pap.Weapon;

namespace VfxEditor.Select.Pap {
    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "pap", manager, isSourceDialog ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new WeaponTab( this, "Weapon" ),
                new ActionTab( this, "Action" ),
                new NonPlayerActionTab( this, "Non-Player Action" ),
                new EmoteTab( this, "Emote" ),
                new NpcPapTab( this, "Npc" ),
                new MountTab( this, "Mount" ),
                new CharacterPapTab( this, "Character" ),
                new JobTab( this, "Job" ),
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
