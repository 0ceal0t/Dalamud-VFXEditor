using VfxEditor.ScdFormat;
using VfxEditor.Select.Scd.Bgm;
using VfxEditor.Select.Scd.BgmQuest;
using VfxEditor.Select.Scd.Common;
using VfxEditor.Select.Scd.Instance;
using VfxEditor.Select.Scd.Mount;
using VfxEditor.Select.Scd.Orchestrion;
using VfxEditor.Select.Scd.Voice;
using VfxEditor.Select.Scd.Zone;

namespace VfxEditor.Select.Scd {
    public class ScdSelectDialog : SelectDialog {
        public ScdSelectDialog( string id, ScdManager manager, bool isSourceDialog ) : base( id, "scd", manager, isSourceDialog ) {
            GameTabs.AddRange( new SelectTab[]{
                new MountScdTab( this, "Mount" ),
                new OrchestrionTab( this, "Orchestrion" ),
                new ZoneTab( this, "Zone" ),
                new BgmTab( this, "BGM" ),
                new BgmQuestTab( this, "Quest BGM" ),
                new InstanceTab( this, "Instance" ),
                new VoiceTab( this, "Voice" ),
                new CommonTab( this, "Common" ),
            } );
        }
    }
}
