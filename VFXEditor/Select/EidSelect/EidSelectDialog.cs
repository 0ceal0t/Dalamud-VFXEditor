using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Select.EidSelect {
    public class EidSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public EidSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "eid", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new EidCharacterSelect( "Character", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
