using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Eid.Character;
using VfxEditor.Select.Eid.Mount;

namespace VfxEditor.Select.Eid {
    public class EidSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public EidSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "eid", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new CharacterTab( this, "Character" ),
                new MountEidTab( this, "Mount" ),
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
