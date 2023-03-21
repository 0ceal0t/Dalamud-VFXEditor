using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Select.ScdSelect {
    public class ScdSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public ScdSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "scd", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ScdMountSelect( "Mount", this ),
                new ScdOrchestrionSelect( "Orchestrion", this ),
                new ScdZoneSelect( "Zone", this ),
                new ScdBgmSelect( "BGM", this ),
                new ScdBgmQuestSelect( "Quest BGM", this ),
                new ScdContentSelect( "Instance", this )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
