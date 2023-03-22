using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select2.Eid.Character;

namespace VfxEditor.Select2.Eid {
    public class EidSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public EidSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "eid", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new CharacterTab( this, "Character" )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
