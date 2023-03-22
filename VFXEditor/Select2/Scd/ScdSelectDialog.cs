using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select2.Scd.Bgm;
using VfxEditor.Select2.Scd.BgmQuest;
using VfxEditor.Select2.Scd.Instance;
using VfxEditor.Select2.Scd.Mount;
using VfxEditor.Select2.Scd.Orchestrion;
using VfxEditor.Select2.Scd.Zone;

namespace VfxEditor.Select2.Scd {
    public class ScdSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public ScdSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "scd", manager, isSourceDialog ) {
            GameTabs = new List<SelectTab>( new SelectTab[]{
                new MountTab( this, "Mount" ),
                new OrchestrionTab( this, "Orchestrion" ),
                new ZoneTab( this, "Zone" ),
                new BgmTab( this, "BGM" ),
                new BgmQuestTab( this, "Quest BGM" ),
                new InstanceTab( this, "Instance" )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
