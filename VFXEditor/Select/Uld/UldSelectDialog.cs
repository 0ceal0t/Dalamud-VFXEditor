using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Uld.Common;

namespace VfxEditor.Select.Uld {
    public class UldSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public UldSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "uld", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new CommonTab( this, "Common" ),
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
